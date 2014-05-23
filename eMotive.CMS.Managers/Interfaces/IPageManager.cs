using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Pages;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Interfaces;

namespace eMotive.CMS.Managers.Interfaces
{
    public interface IPageManager : ISearchable<Page>, IAuditable
    {
        Page New();

        bool Create(Page page, out int id);

        bool Update(Page page);

        bool DeletePage(int id);

        Page FetchPage(int id);

        Page FetchPage(string name);


        Section NewSection();

        bool Create(Section section, out int id);

        bool Update(Section section);

        bool DeleteSection(int id);

        Section FetchSection(int id);

        Section FetchSection(string name);

        IEnumerable<Section> FetchSections();

        IEnumerable<Section> FetchSections(IEnumerable<int> ids);

        IEnumerable<Page> FetchPages();

        IEnumerable<Page> FetchPages(IEnumerable<int> ids);
    }
}
