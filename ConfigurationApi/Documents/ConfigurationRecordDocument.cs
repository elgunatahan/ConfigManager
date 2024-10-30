using ConfigurationApi.Entities;
using ConfigurationApi.Entities.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ConfigurationApi.Documents
{
    public class ConfigurationRecordDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]

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

        public static implicit operator ConfigurationRecord(ConfigurationRecordDocument document)
        {
            if (document == null)
                return null;

            ConfigurationRecord configurationRecord = new ConfigurationRecord(

                environment: document.Environment,
                applicationName: document.ApplicationName,
                key: document.Key,
                value: document.Value,
                type: document.Type,

                isActive: document.IsActive,

                identity: new IdentityValueObject(document.Id, document.Version),
                audit: new AuditValueObject(document.CreatedAt, document.UpdatedAt)
                );

            return configurationRecord;
        }

        public static explicit operator ConfigurationRecordDocument(ConfigurationRecord configurationRecord)
        {
            if (configurationRecord == null)
                return null;

            return new ConfigurationRecordDocument()
            {
                Id = configurationRecord.IdentityObject.Id,
                Version = configurationRecord.IdentityObject.Version,
                IsActive = configurationRecord.IsActive,

                ApplicationName = configurationRecord.ApplicationName,
                Environment = configurationRecord.Environment,
                Key = configurationRecord.Key,
                Value = configurationRecord.Value,
                Type = configurationRecord.Type
            };
        }
    }
}
