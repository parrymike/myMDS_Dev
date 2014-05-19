using System;
using eMotive.CMS.Extensions;
using eMotive.CMS.Repositories.Objects.Users;
using eMotive.CMS.Search.Interfaces;
using Lucene.Net.Documents;

namespace eMotive.CMS.Managers.Objects.Search
{
    public class UserSearchDocument : ISearchDocument
    {
        public UserSearchDocument(User user)
        {
            User = user;
            DatabaseID = user.ID;
            Title = string.Format("{0} {1}", User.Forename, User.Surname);
            Description = String.Empty;
            Type = "User";
            UniqueID = string.Format("{0}_{1}", Type, user.ID);
        }

        public int DatabaseID { get; set; }
        public string UniqueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public User User { get; private set; }

        public Document BuildRecord()
        {
            var doc = new Document();

            var numericField = new NumericField("DatabaseID", Field.Store.YES, false);
            numericField.SetIntValue(User.ID);
            doc.Add(numericField);

            var field = new Field("UniqueID", UniqueID, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Title", Title, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Description", Description, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Type", Type, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);

            field = new Field("Username", User.Username, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);

            field = new Field("Forename", User.Forename, Field.Store.NO, Field.Index.ANALYZED);
            doc.Add(field);

            field = new Field("Surname", User.Surname, Field.Store.NO, Field.Index.ANALYZED);
            doc.Add(field);

            field = new Field("EventDescription", User.Email, Field.Store.NO, Field.Index.ANALYZED);
            doc.Add(field);

            if (User.Roles.HasContent())
            {
                foreach (var role in User.Roles)
                {
                    field = new Field("Role", role.Name, Field.Store.NO, Field.Index.ANALYZED);
                    doc.Add(field);
                }
            }

            field = new Field("Enabled", User.Enabled.ToString(), Field.Store.NO, Field.Index.ANALYZED);
            doc.Add(field);

            field = new Field("Archived", User.Archived.ToString(), Field.Store.NO, Field.Index.ANALYZED);
            doc.Add(field);

            return doc;
        }
    }
}
