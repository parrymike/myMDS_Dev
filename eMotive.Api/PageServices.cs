using System.Collections.Generic;
using System.Linq;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Json;
using eMotive.CMS.Models.Objects.Pages;
using eMotive.CMS.Models.Objects.Roles;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace eMotive.Api.Services.Pages
{
    //TODO: reverse this, so /sections/pages rather than /pages/sections ??
    [Route("/Pages/Sections/New", "GET")]
    public class NewSection
    {
    }

    [Route("/Pages/New", "GET")]
    public class NewPage
    {
    }

    [Route("/Pages/Audit", "GET")]
    public class GetAudit
    {
        public int Id { get; set; }

    }

    [Route("/Pages/Audit/RollBack", "POST")]
    public class RollBack
    {
        public AuditRecord record { get; set; }
    }


    [Route("/Pages")]
    [Route("/Pages/{Ids}")]
    public class GetPages
    {
        public int[] Ids { get; set; }
    }

    [Route("/Pages/Sections")]
    [Route("/Pages/Sections/{Ids}")]
    public class GetSections
    {
        public int[] Ids { get; set; }
    }

    [Route("/Pages/Sections", "DELETE")]
    public class DeleteSection
    {
        public int Id { get; set; }
    }

    [Route("/Pages/Sections", "POST")]
    [Route("/Pages/Sections", "PUT")]
    public class SaveSection
    {
        public Section Section { get; set; }
    }

    [Route("/Pages", "POST")]
    [Route("/Pages", "PUT")]
    public class SavePage
    {
        public Page Page { get; set; }
    }

    [Route("/Pages", "DELETE")]
    public class DeletePage
    {
        public int Id { get; set; }
    }

    [Route("/Pages/Search", "POST")]
    public class DoSearch
    {
        public RoleSearch RoleSearch { get; set; }
    }

    //http://stackoverflow.com/questions/15231537/recommended-servicestack-api-structure/15235822#15235822
    public class PageService : Service
    {
        private readonly IPageManager _pageManager;
        private readonly IMessageBusService _messageBusService;
        private readonly IAuditService _auditService;

        public PageService(IPageManager pageManager, IMessageBusService messageBusService, IAuditService auditService)
        {
            _pageManager = pageManager;
            _messageBusService = messageBusService;
            _auditService = auditService;
        }

        public object Get(NewSection request)
        {
            return new ServiceResult<Section>
            {
                Success = true,
                Result = _pageManager.NewSection(),
                Errors = new string[] { }
            };

        }

        public object Get(NewPage request)
        {
            return new ServiceResult<Page>
            {
                Success = true,
                Result = _pageManager.New(),
                Errors = new string[] { }
            };

        }

        public object Post(RollBack request)
        {
            var success = _pageManager.RollBack(request.record);

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<bool>
            {
                Success = success,
                Result = success,
                Errors = issues
            };

        }

        public object Get(GetAudit request)
        {
            var result = _auditService.FetchLog<Page>(request.Id);

            var success = result.HasContent();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<AuditRecord>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }
        /*
        public object Post(DoSearch request)
        {

            var searchItem = _emailService.DoSearch(request.RoleSearch);

            if (searchItem.Items.HasContent())
            {
                request.RoleSearch.NumberOfResults = searchItem.NumberOfResults;
                request.RoleSearch.Roles = _emailService.FetchRecordsFromSearch(searchItem);
            }

            var success = request.RoleSearch.Roles.HasContent();

            var issues = MessageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<RoleSearch>
            {
                Success = success,
                Result = request.RoleSearch,
                Errors = issues
            };

        }*/

        public object Get(GetSections request)
        {
            var result = request.Ids.IsEmpty()
                ? _pageManager.FetchSections()
                : _pageManager.FetchSections(request.Ids);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<Section>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }

        public object Post(SaveSection request)
        {
            int id;
            var success = _pageManager.Create(request.Section, out id);

            if (success)
                request.Section.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Section>
            {
                Success = success,
                Result = request.Section,
                Errors = issues
            };
        }

        public object Put(SaveSection request)
        {
            var success = _pageManager.Update(request.Section);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Section>
            {
                Success = success,
                Result = request.Section,
                Errors = issues
            };
        }

        public object Delete(DeleteSection request)
        {
            var success = _pageManager.DeleteSection(request.Id);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

        //    if (success)
             //   request.Course = null;

            return new ServiceResult<bool>
            {
                Success = success,
                Result = success,
                Errors = issues
            };
        }

        public object Get(GetPages request)
        {
            var result = request.Ids.IsEmpty()
                ? _pageManager.FetchPages()
                : _pageManager.FetchPages(request.Ids);

            var success = !result.IsEmpty();

            var issues = _messageBusService.Fetch().Select(m => m.Details); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<IEnumerable<Page>>
            {
                Success = success,
                Result = result,
                Errors = issues
            };

        }

        public object Post(SavePage request)
        {
            int id;
            var success = _pageManager.Create(request.Page, out id);

            if (success)
                request.Page.ID = id;

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Page>
            {
                Success = success,
                Result = request.Page,
                Errors = issues
            };
        }

        public object Put(SavePage request)
        {
            var success = _pageManager.Update(request.Page);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            return new ServiceResult<Page>
            {
                Success = success,
                Result = request.Page,
                Errors = issues
            };
        }

        public object Delete(DeletePage request)
        {
            var success = _pageManager.DeletePage(request.Id);

            var issues = _messageBusService.Fetch().Select(m => m.Details); ;

            //    if (success)
            //   request.Course = null;

            return new ServiceResult<bool>
            {
                Success = success,
                Result = success,
                Errors = issues
            };
        }

    }
}