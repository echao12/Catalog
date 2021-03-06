using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Catalog.Repositories;
using Catalog.Settings;
using MongoDB.Driver;
using MongoDB.Bson;//Bson
using MongoDB.Bson.Serialization;//BsonSerializer
using MongoDB.Bson.Serialization.Serializers;//GuidSerializer
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace Catalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //makes Guid & datetimeoffset types friendlier to use with mongoDb
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));//if MongoDB sees a Guid in the entities, convert it to a string in the database.
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));//same thing for the DateTimeOffset types in the entities.

            //register dependency!
            //singleton has only ONE copy of an instance of a type during the lifetime of the program.
            
            //take a serviceprovider.
            //grab settings infomation from our MongoDB settings class.
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            services.AddSingleton<IMongoClient>(serviceProvider => {
                //GetSection looks for the section "MongoDbSettings" under appsettings.json file.
                //note: using nameof() b/c the class name "MongoDbSettings" is the same as the section name.
                //      GetSection returns a IConfigurationSection type & Get<>() binds it to the <type> speficied.
                
                return new MongoClient(mongoDbSettings.ConnectionString);
            });

            //swapped dependencies to use the MongoDb
            services.AddSingleton<IItemsRepository, MongoDbItemsRepository>(); //InMemItemsRepository>(); //<interface, Instance>
            //note: ASP.Net Core 3.0+, the framework removes "Async" suffix from fns internally.
            // ex: GetItemsAsync == GetItems    internally.
            services.AddControllers(options => {
                // we want to remove that behavior b/c we are using fn names to map to types in this
                // program with nameof().
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });

            //add a mongodb health check
            services.AddHealthChecks()
                .AddMongoDb(
                    mongoDbSettings.ConnectionString, 
                    name: "mongodb", 
                    timeout: TimeSpan.FromSeconds(3),
                    tags: new[] { "ready" }
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }

            if (env.IsDevelopment()){
                //uses redirection only if in development environment.
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //map this route to perform healthchecks to see if the database is responsive by performing the healthchecks with the "ready" tag.
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions{
                    Predicate = (check) => check.Tags.Contains("ready"), //so far, checks if database is ready for use
                    //generate custom response to render for this health check with response writer
                    ResponseWriter = async(context, report) => {
                        //create a new json string
                        var result = JsonSerializer.Serialize(
                            //shape the response we want to get back
                            new {
                                status = report.Status.ToString(), // show status
                                //show an array of checks by projecting each entry into a new anonymous type
                                checks = report.Entries.Select(entry => new {
                                    name = entry.Key, // show the name
                                    status = entry.Value.Status.ToString(), //status of this check
                                    exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none", // not a null exception(there is an exception)? show msg. else show "none" 
                                    duration = entry.Value.Duration.ToString() // how long the health check took
                                })
                            }
                        );
                        //format the output
                        context.Response.ContentType = MediaTypeNames.Application.Json; // tells response to render as a json
                        await context.Response.WriteAsync(result); //output the information to screen
                    }
                });//maps health checks to specified route. it can be whatever.

                //map this route to perform healthcheck to see if server is online.
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions{
                    Predicate = (_) => false // maps ignores all predicates/tags and essentially acts as a ping test to the server.
                });//maps health checks to specified route. it can be whatever.
            });
        }
    }
}
