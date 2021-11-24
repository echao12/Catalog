namespace Catalog.Settings {
    public class MongoDbSettings {
        public string Host {get; set;}
        public int Port {get; set;}
        public string User {get; set;} // will be populated by .NET during runtime
        public string Password {get; set;} // will be populated by .NET during runtime
        public string ConnectionString { 
            get {
                //format mongo is expecting
                return $"mongodb://{User}:{Password}@{Host}:{Port}";
            }
        }
    }
}