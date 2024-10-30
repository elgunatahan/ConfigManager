using System;

namespace ConfigurationApi.Models.Responses
{
    public class GetConfigurationRecordResponse
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Environment { get; set; }
        public string ApplicationName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
