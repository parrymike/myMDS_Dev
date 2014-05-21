using System;
using System.Collections.Generic;
using AutoMapper;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Objects;
using eMotive.CMS.Services.Objects.Audit;
using Rep = eMotive.CMS.Repositories.Objects;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Pages;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Interfaces;
using Map = eMotive.CMS.Managers.AutoMapperConfiguration.Maps;
using emSearch = eMotive.CMS.Search.Objects.Search;

namespace eMotive.CMS.Managers.Objects.Managers
{
    public class PageManager : IPageManager
    {
        private readonly IPageRepository _pageRepository;

        public PageManager(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
            AutoMapperConfiguration.Configure(Map.Page);
        }

        public IMappingEngine Mapper { get; set; }
        public IEventManagerService EventManagerService { get; set; }
        public ISearchManager SearchManager { get; set; }
        public IMessageBusService MessageBusService { get; set; }
        public IAuditService AuditService { get; set; }

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

        public bool DeletePage(int id)
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

        public Section NewSection()
        {
            return Mapper.Map<Rep.Pages.Section, Section>(_pageRepository.NewSection());
        }

        public bool Create(Section section, out int id)
        {
            id = -1;

            var checkSection = _pageRepository.FetchSection(section.Name);

            if (checkSection != null)
            {
                MessageBusService.AddIssue(string.Format("A section with the name '{0}' already exists.", section.Name));
                return false;
            }

            var repSection = Mapper.Map<Section, Rep.Pages.Section>(section);

            if (_pageRepository.Create(repSection, out id))
            {
                var newSection = FetchSection(id);
                AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, newSection);

               // EventManagerService.QueueEvent(new SectionCreatedEvent(section));

                return true;
            }
            MessageBusService.AddIssue("An error occurred. The section has not been created.");
            return false;
        }

        public bool Put(Section section)
        {
            throw new NotImplementedException();
        }

        public bool Update(Section section)
        {
            throw new NotImplementedException();
        }

        public bool DeleteSection(int id)
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

        public SearchResult DoSearch(BasicSearch search)
        {
            throw new NotImplementedException();
        }

        public void ReindexSearchRecords()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Page> FetchRecordsFromSearch(SearchResult searchResult)
        {
            throw new NotImplementedException();
        }

        public bool RollBack(AuditRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
