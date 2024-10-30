using System;

namespace ConfigurationApi.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }

        public string Data { get; set; }

        public DateTime OccurredOn { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public string Type { get; set; }
    }
}
