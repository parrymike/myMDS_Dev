using System;
using eMotive.CMS.Repositories.Objects.Users;
using eMotive.CMS.Search.Interfaces;
using Lucene.Net.Documents;

namespace eMotive.CMS.Managers.Objects.Search
{
    public class RoleSearchDocument : ISearchDocument
    {
        public RoleSearchDocument(Role role)
        {
            Role = role;
            DatabaseID = role.ID;
            Title = role.Name;
            Description = String.Empty;
            Type = "Role";
            UniqueID = string.Format("{0}_{1}", Type, role.ID);
        }

        public int DatabaseID { get; set; }
        public string UniqueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        private Role Role { get; set; }

        public Document BuildRecord()
        {
            var doc = new Document();

            var numericField = new NumericField("DatabaseID", Field.Store.YES, false);
            numericField.SetIntValue(Role.ID);
            doc.Add(numericField);

            var field = new Field("UniqueID", UniqueID, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Title", Title, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Description", Description, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Type", Type, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);

            field = new Field("Name", Role.Name, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);

            return doc;
        }
    }
}
