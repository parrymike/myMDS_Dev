using eMotive.CMS.Search.Objects;

namespace eMotive.CMS.Search.Interfaces
{
    public interface ISearchManager
    {
        SearchResult DoSearch(Objects.Search search);

        bool Add(ISearchDocument document);
        bool Update(ISearchDocument document);
        bool Delete(ISearchDocument document);

        void DeleteAll();

        int NumberOfDocuments();
    }
}
