using Lucene.Net.Documents;

namespace eMotive.CMS.Search.Interfaces
{
    public interface ISearchDocument
    {
        int DatabaseID { get; set; }
        string UniqueID { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Type { get; set; }

        Document BuildRecord();
    }
}
