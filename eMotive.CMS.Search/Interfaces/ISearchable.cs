using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Search.Objects;

namespace eMotive.CMS.Search.Interfaces
{
    public interface ISearchable<out T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        SearchResult DoSearch(BasicSearch search);
        /// <summary>
        /// To pull items from their datastore and reindex them in lucene.
        /// </summary>
        void ReindexSearchRecords();

        IEnumerable<T> FetchRecordsFromSearch(SearchResult searchResult);
    }
}
