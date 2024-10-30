using System;

namespace ConfigurationApi.Events
{
    public class ConfigurationRecordDeleted
    {
        public string Environment { get; set; }
        public string ApplicationName { get; set; }
        public string Key { get; set; }
        public Guid ConfigurationRecordId { get; set; }

    }
}
