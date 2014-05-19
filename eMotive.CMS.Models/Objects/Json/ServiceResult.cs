using System.Collections.Generic;

namespace eMotive.CMS.Models.Objects.Json
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Result { get; set; }
    }
}
