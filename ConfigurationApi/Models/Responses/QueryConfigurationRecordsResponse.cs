using System;
using System.Collections.Generic;

namespace ConfigurationApi.Models.Responses
{
    public class QueryConfigurationRecordsResponse
    {
        public List<ConfigurationRecordItemResponse> Items { get; set; }
        public int TotalRecords { get; set; }
    }
    public class ConfigurationRecordItemResponse
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
