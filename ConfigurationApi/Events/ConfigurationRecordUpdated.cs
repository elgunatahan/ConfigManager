using System;

namespace ConfigurationApi.Events
{
    public class ConfigurationRecordUpdated
    {
        public Guid ConfigurationRecordId { get; set; }

        public int Version { get; set; }
    }
}
