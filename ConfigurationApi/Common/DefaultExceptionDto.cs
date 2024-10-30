using System.Collections.Generic;

namespace ConfigurationApi.Common
{
    public class DefaultExceptionDto
    {
        public List<string> Errors { get; set; }
        public string Details { get; set; }

        public DefaultExceptionDto()
        {
            Errors = new List<string>();
        }

        public int ErrorCode { get; set; }
    }
}
