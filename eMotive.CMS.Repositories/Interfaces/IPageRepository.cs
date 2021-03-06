﻿using System.Collections.Generic;
using eMotive.CMS.Repositories.Objects.Pages;

namespace eMotive.CMS.Repositories.Interfaces
{
    public interface IPageRepository
    {
        Page New();

        bool Create(Page page, out int id);

        bool Put(Page page);

        bool Update(Page page);

        bool Delete(Page page);

        Page FetchPage(int id);

        Page FetchPage(string name);


        Section NewSection();

        bool Create(Section section, out int id);

        bool Update(Section section);

        bool Delete(Section section);

        Section FetchSection(int id);

        Section FetchSection(string name);

        IEnumerable<Section> FetchSections();

        IEnumerable<Section> FetchSections(IEnumerable<int> ids);

        IEnumerable<Page> FetchPages();

        IEnumerable<Page> FetchPages(IEnumerable<int> ids);
    }
}
