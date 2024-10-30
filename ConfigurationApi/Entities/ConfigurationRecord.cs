using ConfigurationApi.Entities.ValueObjects;

namespace ConfigurationApi.Entities
{
    public class ConfigurationRecord : BaseEntity
    {
        public bool IsActive { get; private set; }
        public string Environment { get; private set; }
        public string ApplicationName { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public string Type { get; private set; }

        public ConfigurationRecord(
            string environment,
            string applicationName,
            string key,
            string value,
            string type,
            bool isActive = true,
            IdentityValueObject identity = null,
            AuditValueObject audit = null
            ) : base(identity)
        {
            IsActive = isActive;
            Audit = audit;

            Environment = environment;
            ApplicationName = applicationName;
            Key = key;
            Value = value;
            Type = type;
        }

        public void Update(string value, string type)
        {
            Value = value.Trim();
            Type = type.Trim();
        }

        public void Delete()
        {
            IsActive = false;
        }
    }
}
