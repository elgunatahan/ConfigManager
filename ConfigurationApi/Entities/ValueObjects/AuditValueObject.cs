using System;

namespace ConfigurationApi.Entities.ValueObjects
{
    public class AuditValueObject
    {
        public DateTime? CreatedAt { get; }
        public DateTime? UpdatedAt { get; }

        public AuditValueObject(AuditValueObject audit)
        {
            CreatedAt = audit.CreatedAt;
        }

        public AuditValueObject(DateTime createdAt, DateTime? updatedAt)
        {
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
