using System;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Services.Objects.EventManagerService;
using Lucene.Net.Documents;

namespace eMotive.CMS.Services.Objects.Search
{
    public class EventSearchDocument : ISearchDocument
    {
        public EventSearchDocument(EventDescription eventDescription)
        {
            EventDescription = eventDescription;
            DatabaseID = eventDescription.ID;
            Title = eventDescription.NiceName;
            Description = String.Empty;
            Type = "EventDescription";
            UniqueID = string.Format("{0}_{1}", Type, eventDescription.ID);
        }

        public int DatabaseID { get; set; }
        public string UniqueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        private EventDescription EventDescription { get; set; }

        public Document BuildRecord()
        {
            var doc = new Document();

            var numericField = new NumericField("DatabaseID", Field.Store.YES, false);
            numericField.SetIntValue(EventDescription.ID);
            doc.Add(numericField);

            var field = new Field("UniqueID", UniqueID, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Title", Title, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Description", Description, Field.Store.YES, Field.Index.NOT_ANALYZED);
            doc.Add(field);

            field = new Field("Type", Type, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);

            field = new Field("Name", EventDescription.Name, Field.Store.YES, Field.Index.ANALYZED);
            doc.Add(field);

            return doc;
        }
    }
}
