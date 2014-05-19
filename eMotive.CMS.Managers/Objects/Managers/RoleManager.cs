using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Managers.Objects.Search;
using eMotive.CMS.Models.Objects.Roles;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects;
using eMotive.CMS.Services.Objects.Audit;
using Lucene.Net.Search;
using rep = eMotive.CMS.Repositories.Objects.Users;
using emSearch = eMotive.CMS.Search.Objects.Search;

namespace eMotive.CMS.Managers.Objects.Managers
{
    public class RoleManager : IRoleManager
    {
        private readonly IRoleRepository _roleRepository;

        public RoleManager(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository; 

            AutoMapperConfiguration.Configure(AutoMapperConfiguration.Maps.Role);

           // ReindexSearchRecords();
        }

        public IMappingEngine Mapper { get; set; }
        public IEventManagerService EventManagerService { get; set; }
        public ISearchManager SearchManager { get; set; }
        public IMessageBusService MessageBusService { get; set; }
        public IAuditService AuditService { get; set; }

        public Role New()
        {
            return Mapper.Map<rep.Role, Role>(_roleRepository.New());
        }

        public Role Fetch(int id)
        {
            throw new NotImplementedException();
        }

        public Role Fetch(string name)
        {
            
            throw new NotImplementedException();
        }

        public bool Create(Role role, out int id)
        {
            var checkRole = _roleRepository.Fetch(role.Name);

            if (checkRole != null)
            {
                if (String.Equals(role.Name, checkRole.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBusService.AddIssue(string.Format("A role with the name '{0}' already exists.", role.Name));
                    id = -1;
                    return false;
                }
            }
            var repRole = Mapper.Map<Role, rep.Role>(role);
            if (_roleRepository.Create(repRole))
            {//TODO: why pull role out her, why not out id???
                var newRole = _roleRepository.Fetch(role.Name);
                role.ID = newRole.ID;

                SearchManager.Add(new RoleSearchDocument(newRole));
                AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, role);

                id = newRole.ID;
                return true;
            }

            MessageBusService.AddError("An error occured while trying to create the role. An administrator has been notified of this issue.");
            id = -1;
            return false;
        }

        private bool Update(Role role, bool isRoleback)
        {
            var checkRole = _roleRepository.Fetch(role.Name);

            if (checkRole != null)
            {
                if (String.Equals(role.Name, checkRole.Name, StringComparison.InvariantCultureIgnoreCase) && role.ID != checkRole.ID)
                {
                    MessageBusService.AddIssue(string.Format("A role with the name '{0}' already exists.", role.Name));
                    return false;
                }
            }
            var repRole = Mapper.Map<Role, rep.Role>(role);

            if (_roleRepository.Update(repRole))
            {
                if(!isRoleback)
                    AuditService.ObjectAuditLog(ActionType.Update, n => n.ID, role);

                //   SearchManager.Update(new RoleSearchDocument(repRole));
                return true;
            }

            MessageBusService.AddError("An error occured while trying to edit the role. An administrator has been notified of this issue.");

            return false;
        }

        public bool Update(Role role)
        {
            return Update(role, false);
        }

        public bool Delete(Role role)
        {
            var roleEmpty = _roleRepository.FindUsersInRole(role.ID);

            if (roleEmpty.HasContent())
            {
                MessageBusService.AddIssue(string.Format("'{0}' is assigned to {1} users. The role could not be deleted.", role.Name, roleEmpty.Count()));
                return false;
            }

            var repRole = Mapper.Map<Role, rep.Role>(role);
            if (_roleRepository.Delete(repRole.ID))
            {
                AuditService.ObjectAuditLog(ActionType.Delete, n => n.ID, role);
             //   SearchManager.Delete(new RoleSearchDocument(repRole));
                return true;
            }

            MessageBusService.AddError("An error occured while trying to edit the role. An administrator has been notified of this issue.");

            return false;
        }

        public IEnumerable<Role> FetchAll()
        {
            return Mapper.Map<IEnumerable<rep.Role>, IEnumerable<Role>>(_roleRepository.FetchAll());
        }

        public IEnumerable<Role> Fetch(IEnumerable<int> ids)
        {
            return Mapper.Map<IEnumerable<rep.Role>, IEnumerable<Role>>(_roleRepository.Fetch(ids));
        }

        public bool RollBack(AuditRecord record)
        {
            var rollBackRole = record.Object.FromJson<Role>();
            var success = Update(rollBackRole, true);

            if (success)
            {
                AuditService.ObjectAuditLog(ActionType.RollBack, n => n.ID, rollBackRole, record);
            }

            return success;
        }

        public SearchResult DoSearch(BasicSearch search)
        {
            var newSearch = AutoMapper.Mapper.Map<BasicSearch, emSearch>(search);
            if (string.IsNullOrEmpty(search.Query))
            {
                newSearch.CustomQuery = new Dictionary<string, emSearch.SearchTerm>
                {
                    {"Type", new emSearch.SearchTerm {Field = "Role", Term = Occur.SHOULD}}
                };
            }
            else
            {
                newSearch.CustomQuery = new Dictionary<string, emSearch.SearchTerm>
                {
                    {"Name", new emSearch.SearchTerm {Field = search.Query, Term = Occur.SHOULD}}
                };
            }
            return SearchManager.DoSearch(newSearch);
        }

        public void ReindexSearchRecords()
        {
            var records = _roleRepository.FetchAll();

            if (!records.HasContent())
            {
                //todo: send an error message here
                return;
            }

            foreach (var item in records)
            {
                SearchManager.Add(new RoleSearchDocument(item));
            }
        }

        public IEnumerable<Role> FetchRecordsFromSearch(SearchResult searchResult)
        {
            if (!searchResult.Items.IsEmpty())
            {
                var repItems = _roleRepository.Fetch(searchResult.Items.Select(n => n.ID).ToList());
                if (repItems.HasContent())
                {
                    return AutoMapper.Mapper.Map<IEnumerable<rep.Role>, IEnumerable<Role>>(repItems);

                }
            }

            return null;
        }
    }
}
