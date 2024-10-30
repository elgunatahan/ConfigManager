using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ConfigurationLibrary.Entities
{
    public class ConfigurationRecord
    {
        [BsonId]
        public Guid Id { get; set; }
        public int Version { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public string Environment { get; set; }
        public string ApplicationName { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

    }
}
