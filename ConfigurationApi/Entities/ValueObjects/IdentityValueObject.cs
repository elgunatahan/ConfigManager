using System;
using System.Collections.Generic;

namespace ConfigurationApi.Entities.ValueObjects
{
    public class IdentityValueObject : BaseValueObject
    {
        public const int NextVersionIncrement = 1;

        public IdentityValueObject(Guid id, int version)
        {
            Id = id;
            Version = version;
        }
        public IdentityValueObject()
        {
            Id = Guid.NewGuid();
            Version = 0;
        }

        public void SetNextVersionNumber()
        {
            Version += NextVersionIncrement;
        }

        public Guid Id { get; }
        public int Version { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Version;
        }
    }
}
