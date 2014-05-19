using System.Collections.Generic;

namespace eMotive.CMS.Search.Objects
{
    public class SearchResult : Search
    {
        public virtual IEnumerable<ResultItem> Items { get; set; }
    }
}
