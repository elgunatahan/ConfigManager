namespace ConfigurationApi.Models.Requests
{
    public class CreateConfigurationRecordRequest
    {
        public string Environment { get; set; }
        public string ApplicationName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
