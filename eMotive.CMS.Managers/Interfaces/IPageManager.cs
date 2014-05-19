﻿using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Pages;

namespace eMotive.CMS.Managers.Interfaces
{
    public interface IPageManager
    {
        Page New();

        bool Create(Page page, out int id);

        bool Put(Page page);

        bool Update(Page page);

        bool Delete(Page page);

        Page FetchPage(int id);

        Page FetchPage(string name);


        bool Create(Section section);

        bool Put(Section section);

        bool Update(Section section);

        bool Delete(Section section);

        Section FetchSection(int id);

        Section FetchSection(string name);

        IEnumerable<Section> Fetch();

        IEnumerable<Section> Fetch(IEnumerable<int> ids);
    }
}