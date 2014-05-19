using System;
using System.Collections.Generic;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Pages;

namespace eMotive.CMS.Managers.Objects.Managers
{
    public class PageManager : IPageManager
    {
        public Page New()
        {
            throw new NotImplementedException();
        }

        public bool Create(Page page, out int id)
        {
            throw new NotImplementedException();
        }

        public bool Put(Page page)
        {
            throw new NotImplementedException();
        }

        public bool Update(Page page)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Page page)
        {
            throw new NotImplementedException();
        }

        public Page FetchPage(int id)
        {
            throw new NotImplementedException();
        }

        public Page FetchPage(string name)
        {
            throw new NotImplementedException();
        }

        public bool Create(Section section)
        {
            throw new NotImplementedException();
        }

        public bool Put(Section section)
        {
            throw new NotImplementedException();
        }

        public bool Update(Section section)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Section section)
        {
            throw new NotImplementedException();
        }

        public Section FetchSection(int id)
        {
            throw new NotImplementedException();
        }

        public Section FetchSection(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Section> Fetch()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Section> Fetch(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
