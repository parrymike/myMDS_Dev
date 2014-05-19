﻿using System.Collections.Generic;
using eMotive.CMS.Models.Objects.Users;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;

namespace eMotive.CMS.Managers.Interfaces
{
    public interface IUserManager : ISearchable<User>
    {
        User New();
        User Fetch(int id);
        IEnumerable<User> FetchAll();
        User Fetch(string username);

        string FetchUserNotes(string username);
        bool SaveUserNotes(string username, string notes);

        IEnumerable<User> Fetch(IEnumerable<int> ids);
        IEnumerable<User> Fetch(IEnumerable<string> usernames);

        bool Create(User user, out int id);//todo: why not widen message bus to spit out created objects etc?? Would save having horrid out ints from Rep through to site / api
        bool Create(User user, IEnumerable<int> groupIds);
        bool Update(User user);
        bool Delete(User user);

     //   IEnumerable<User> FetchRecordsFromSearch(SearchResult searchResult);
    }
}
