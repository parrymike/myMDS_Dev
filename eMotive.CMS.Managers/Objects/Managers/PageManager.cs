using System;
using System.Collections.Generic;
using AutoMapper;
using eMotive.CMS.Extensions;
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
            return Mapper.Map<Rep.Pages.Page, Page>(_pageRepository.New());
        }

        public bool Create(Page page, out int id)
        {
            id = -1;

            //todo: need to check page in this section only
            var checkPage = _pageRepository.FetchPage(page.Name);

            if (checkPage != null)
            {
                MessageBusService.AddIssue(string.Format("A page with the name '{0}' already exists in this section."));
                return false;
            }

            var repPage = Mapper.Map<Page, Rep.Pages.Page>(page);

            if (_pageRepository.Create(repPage, out id))
            {
                var newPage = FetchPage(id);
                AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, newPage);

                // EventManagerService.QueueEvent(new SectionCreatedEvent(section));

                return true;
            }
            MessageBusService.AddIssue("An error occurred. The section has not been created.");
            return false;
        }

        public bool Update(Page page)
        {
            var checkPage = _pageRepository.FetchSection(page.Name);

            if (checkPage != null)
            {
                if (String.Equals(page.Name, checkPage.Name, StringComparison.InvariantCultureIgnoreCase) && page.ID != checkPage.ID)
                {
                    MessageBusService.AddIssue(string.Format("A page with the name '{0}' already exists in this section.", page.Name));
                    return false;
                }
            }

            var repPage = Mapper.Map<Page, Rep.Pages.Page>(page);

            if (_pageRepository.Update(repPage))
            {
                //   var updated = Fetch(course.ID);
                AuditService.ObjectAuditLog(ActionType.Update, n => n.ID, page);
                 //  EventManagerService.QueueEvent(new Page(updated));

                return true;
            }

            MessageBusService.AddIssue("An error occurred. The section was not updated.");

            return false;
        }

        public bool DeletePage(int id)
        {
            var page = FetchPage(id);
            return _pageRepository.Delete(Mapper.Map<Page, Rep.Pages.Page>(page));
        }

        public Page FetchPage(int id)
        {
            return Mapper.Map<Rep.Pages.Page, Page>(_pageRepository.FetchPage(id));
        }

        public Page FetchPage(string name)
        {
            return Mapper.Map<Rep.Pages.Page, Page>(_pageRepository.FetchPage(name));
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
                //var newSection = FetchSection(id);
             //   AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, newSection);

               // EventManagerService.QueueEvent(new SectionCreatedEvent(section));

                return true;
            }
            MessageBusService.AddIssue("An error occurred. The section has not been created.");
            return false;
        }

        public bool Update(Section section)
        {
            var checkSection = _pageRepository.FetchSection(section.Name);

            if (checkSection != null)
            {
                if (String.Equals(section.Name, checkSection.Name, StringComparison.InvariantCultureIgnoreCase) && section.ID != checkSection.ID)
                {
                    MessageBusService.AddIssue(string.Format("A section with the name '{0}' already exists.", section.Name));
                    return false;
                }
            }

            var repSection = Mapper.Map<Section, Rep.Pages.Section>(section);

            if (_pageRepository.Update(repSection))
            {
                //   var updated = Fetch(course.ID);
                //AuditService.ObjectAuditLog(ActionType.Update, n => n.ID, updated);
                //   EventManagerService.QueueEvent(new SectionUpdatedEvent(updated));

                return true;
            }

            MessageBusService.AddIssue("An error occurred. The section was not updated.");

            return false;
            
        }

        public bool DeleteSection(int id)
        {
            throw new NotImplementedException();
        }

        public Section FetchSection(int id)
        {
            return Mapper.Map<Rep.Pages.Section, Section>(_pageRepository.FetchSection(id));
        }

        public Section FetchSection(string name)
        {
            return Mapper.Map<Rep.Pages.Section, Section>(_pageRepository.FetchSection(name));
        }

        public IEnumerable<Section> FetchSections()
        {
            return Mapper.Map<IEnumerable<Rep.Pages.Section>, IEnumerable<Section>>(_pageRepository.FetchSections());
        }

        public IEnumerable<Section> FetchSections(IEnumerable<int> ids)
        {
            return Mapper.Map<IEnumerable<Rep.Pages.Section>, IEnumerable<Section>>(_pageRepository.FetchSections(ids));
        }

        public IEnumerable<Page> FetchPages()
        {
            return Mapper.Map<IEnumerable<Rep.Pages.Page>, IEnumerable<Page>>(_pageRepository.FetchPages());
        }

        public IEnumerable<Page> FetchPages(IEnumerable<int> ids)
        {
            return Mapper.Map<IEnumerable<Rep.Pages.Page>, IEnumerable<Page>>(_pageRepository.FetchPages(ids));
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
            var rollBackApplication = record.Object.FromJson<Page>();
            var repApp = Mapper.Map<Page, Rep.Pages.Page>(rollBackApplication);
            var success = _pageRepository.Put(repApp);

            if (success)
            {
                AuditService.ObjectAuditLog(ActionType.RollBack, n => n.ID, rollBackApplication, record);
              //  EventManagerService.QueueEvent(new ApplicationRolledBackEvent(rollBackApplication));
            }

            return success;
        }
    }
}
