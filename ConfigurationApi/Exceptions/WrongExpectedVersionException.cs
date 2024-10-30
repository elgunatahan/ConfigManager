using System;

namespace ConfigurationApi.Exceptions
{
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException(string message) : base(message)
        {
        }
    }
}
