using System;
using eMotive.CMS.Repositories.Objects.Users;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Objects.EmailService;
using Lucene.Net.Documents;

namespace eMotive.CMS.Services.Objects.Search
{
    public class EmailSearchDocument : ISearchDocument
    {
        public EmailSearchDocument(Email email)
        {
            Email = email;
            DatabaseID = email.ID;
            Title = email.Subject;
            Description = String.Empty;
            Type = "EventDescription";
            UniqueID = string.Format("{0}_{1}", Type, email.ID);
        }

        public int DatabaseID { get; set; }
        public string UniqueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        private Email Email { get; set; }

        public Document BuildRecord()
        {
            var doc = new Document();

            var numericField = new NumericField("DatabaseID", Field.Store.YES, false);
            numericField.SetIntValue(Email.ID);
            doc.Add(numericField);

            var field = new Field("UniqueID", UniqueID, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Title", Title, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Description", Description, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Type", Type, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);

           /* field = new Field("Name", EventDescription.Name, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);*/

            return doc;
        }
    }
}
