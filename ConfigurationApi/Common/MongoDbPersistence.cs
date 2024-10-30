using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization;
using ConfigurationApi.Documents;

namespace ConfigurationApi.Common
{
    public static class MongoDbPersistence
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<ConfigurationRecordDocument>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });

            var pack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("My Solution Conventions", pack, t => true);
        }
    }
}
