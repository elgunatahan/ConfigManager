using ConfigurationApi.Entities.ValueObjects;

namespace ConfigurationApi.Entities
{
    public class BaseEntity
    {
        public BaseEntity(IdentityValueObject identityObject)
        {
            IdentityObject = identityObject ?? new IdentityValueObject();
        }

        public IdentityValueObject IdentityObject { get; }

        public AuditValueObject Audit { get; protected set; }
    }
}
