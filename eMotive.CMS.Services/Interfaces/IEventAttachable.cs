using System.Collections.Generic;

namespace eMotive.CMS.Services.Interfaces
{
    public interface IEventAttachable<T>
    {
        IEnumerable<T> Fetch(string section);
    }
}
