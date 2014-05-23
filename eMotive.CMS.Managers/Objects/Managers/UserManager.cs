using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Managers.Objects.Search;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Models.Objects.Users;
using eMotive.CMS.Repositories;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects;
using eMotive.CMS.Services.Objects.Audit;
using Lucene.Net.Search;
using Map = eMotive.CMS.Managers.AutoMapperConfiguration.Maps;
using emSearch = eMotive.CMS.Search.Objects.Search;
using repUsers = eMotive.CMS.Repositories.Objects.Users;

namespace eMotive.CMS.Managers.Objects.Managers
{
    public enum CreateUser { Success, Error, DuplicateUsername, DuplicateEmail, Deletedaccount }

    public class UserManager : IUserManager
    {

        private readonly IUserRepository userRep;
        private readonly ISearchManager searchManager;
        private readonly IAccountManager accountManager;

        public UserManager(IUserRepository _userRep)
        {
            userRep = _userRep;

            AutoMapperConfiguration.Configure(Map.User);
        }

        public IMappingEngine Mapper { get; set; }
        public ISearchManager SearchManager { get; set; }
        public IMessageBusService MessageBusService { get; set; }
        public IRoleManager RoleManager { get; set; }
        public IAuditService AuditService { get; set; }

        public User New()
        {
            return Mapper.Map<repUsers.User, User>(userRep.New());
        }

        public User Fetch(int id)
        {
            var repUser = userRep.Fetch(id);

            return Mapper.Map<repUsers.User, User>(repUser);
        }

        public IEnumerable<User> FetchAll()
        {
            return Mapper.Map<IEnumerable<repUsers.User>, IEnumerable<User>>(userRep.FetchAll());
        }

        public User Fetch(string username)
        {
            var repUser = userRep.Fetch(username, FetchByUserField.Username);

            return Mapper.Map<repUsers.User, User>(repUser);
        }

        public string FetchUserNotes(string username)
        {
            return userRep.FetUserNotes(username);
        }

        public bool SaveUserNotes(string username, string notes)
        {
            return userRep.SaveUserNotes(username, notes);
        }

        public IEnumerable<User> Fetch(IEnumerable<int> ids)
        {
            return Mapper.Map<IEnumerable<repUsers.User>, IEnumerable<User>>(userRep.Fetch(ids));
        }

        public IEnumerable<User> Fetch(IEnumerable<string> usernames)
        {
            return Mapper.Map<IEnumerable<repUsers.User>, IEnumerable<User>>(userRep.Fetch(usernames));
        }

        public bool Create(User user, out int id)
        {
            var repUser = Mapper.Map<User, repUsers.User>(user);

            var checkUser = userRep.Fetch(user.Username, FetchByUserField.Username);

            if (checkUser != null)
            {
                if (checkUser.Archived)
                {
                    MessageBusService.AddIssue(GetErrorMessage(CreateUser.Deletedaccount, user.Username));
                    id = -1;
                    return false;
                }

                if (user.Username.ToLowerInvariant() == checkUser.Username.ToLowerInvariant())
                {
                    MessageBusService.AddIssue(GetErrorMessage(CreateUser.DuplicateUsername, user.Username));
                    id = -1;
                    return false;
                }
            }

            checkUser = userRep.Fetch(user.Email, FetchByUserField.Email);

            if (checkUser != null)
            {
                if (user.Email.ToLowerInvariant() == checkUser.Email.ToLowerInvariant())
                {
                    MessageBusService.AddIssue(GetErrorMessage(CreateUser.DuplicateEmail, user.Email));
                    id = -1;
                    return false;
                }
            }

            int createId;
            if (userRep.Create(repUser, out createId))
            {
                user.ID = createId;

                if (accountManager.CreateNewAccountPassword(user))
                {
                    user.ID = repUser.ID = createId;
                    searchManager.Add(new UserSearchDocument(repUser));

                    id = createId;
                    return true;
                }
                id = createId;
                return false;
            }

            MessageBusService.AddError(GetErrorMessage(CreateUser.Error));
            id = -1;
            return false;
        }

        public bool Update(User user)
        {
            var checkUser = userRep.Fetch(user.Username, FetchByUserField.Username);

            if (checkUser != null)
            {
                if (user.Username.ToLowerInvariant() == checkUser.Username.ToLowerInvariant() && user.ID != checkUser.ID)
                {
                    MessageBusService.AddIssue(string.Format("A user with the username '{0}' already exists.", user.Username));
                    return false;
                }
            }

            checkUser = userRep.Fetch(user.Email, FetchByUserField.Email);

            if (checkUser != null)
            {
                if (user.Email.ToLowerInvariant() == checkUser.Email.ToLowerInvariant() && user.ID != checkUser.ID)
                {
                    MessageBusService.AddIssue(string.Format("A user with the email address '{0}' already exists.", user.Email));
                    return false;
                }
            }

            var repUser = Mapper.Map<User, repUsers.User>(user);

            if (userRep.Update(repUser))
            {
                searchManager.Update(new UserSearchDocument(repUser));
                return true;
            }

            MessageBusService.AddError("An error occurred while trying to edit the user. An administrator has been notified of this issue.");

            return false;
        }

        public bool Delete(int id)
        {
            var user = Fetch(id);

            if (userRep.Delete(Mapper.Map<User, repUsers.User>(user)))
            {
                var deletedUser = userRep.Fetch(user.ID);
                searchManager.Update(new UserSearchDocument(deletedUser));
                AuditService.ObjectAuditLog(ActionType.Delete, n => n.ID, user);
                return true;
            }

            MessageBusService.AddError(string.Format("An error occured while trying to delete '{0}'. An administrator has been notified of this issue.", user.Username));

            return false;
        }

        public IEnumerable<User> FetchRecordsFromSearch(SearchResult searchResult)
        {
            if (searchResult.Items.HasContent())
            {
                var repItems = userRep.Fetch(searchResult.Items.Select(n => n.ID).ToList());
                if (repItems.HasContent())
                {
                    return Mapper.Map<IEnumerable<repUsers.User>, IEnumerable<User>>(repItems);

                }
            }

            return null;
        }

        public SearchResult DoSearch(BasicSearch search)
        {
            var newSearch = Mapper.Map<BasicSearch, emSearch>(search);
            if (string.IsNullOrEmpty(search.Query))
            {
                newSearch.CustomQuery = new Dictionary<string, emSearch.SearchTerm>
                {
                    {"Type", new emSearch.SearchTerm {Field = "User", Term = Occur.SHOULD}}
                };
            }
            else
            {
                newSearch.CustomQuery = new Dictionary<string, emSearch.SearchTerm>
                {
                    {"Username", new emSearch.SearchTerm {Field = search.Query, Term = Occur.SHOULD}},
                    {"Forename", new emSearch.SearchTerm {Field = search.Query, Term = Occur.SHOULD}},
                    {"Surame", new emSearch.SearchTerm {Field = search.Query, Term = Occur.SHOULD}},
                    {"Email", new emSearch.SearchTerm {Field = search.Query, Term = Occur.SHOULD}}//,
                    //{"Archived", new emSearch.SearchTerm {Field = "False", Term = Occur.SHOULD}}
                };
            }
            return searchManager.DoSearch(newSearch);
            //  var result = _rawResults = searchManager.DoSearch(newSearch);
            /*
                        if (result.Items.HasContent())
                        {
                            return Mapper.Map <IEnumerable<repUsers.User>,IEnumerable<User>> (userRep.Fetch(result.Items.Select(n => n.ID).ToList()));
                        }*/

            return null;
        }

        public void ReindexSearchRecords()
        {
            var records = userRep.FetchAll();

            if (records.IsEmpty())
            {
                //todo: send an error message here
                return;
            }

            foreach (var item in records)
            {
                searchManager.Add(new UserSearchDocument(item));
            }
        }

        private static string GetErrorMessage(CreateUser message, string field = "")
        {
            switch (message)
            {
                case CreateUser.Deletedaccount:
                    return string.Format("The account for username '{0}' has been deleted.", field);
                case CreateUser.DuplicateUsername:
                    return string.Format("The username '{0}' is unavailable.", field);
                case CreateUser.DuplicateEmail:
                    return string.Format("The email address '{0}' is already registered to an account.", field);
                case CreateUser.Error:
                    goto default;
                default:
                    return "An error occured. An administrator has been notified of this issue.";
            }
        }

        public bool RollBack(AuditRecord record)
        {
            throw new System.NotImplementedException();
        }
    }
}
