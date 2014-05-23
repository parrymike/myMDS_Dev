using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Transactions;
using Dapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Models.Objects.Event;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using eMotive.CMS.Services.Objects.EventManagerService;
using Lucene.Net.Search;
using MySql.Data.MySqlClient;


namespace eMotive.CMS.Services.Objects.Service
{
    public class EventManagerService : IEventManagerService
    {
        internal readonly ICollection<IEvent> Events;

        private readonly string _connectionString;
        private IDbConnection _connection;

        public EventManagerService(string connectionString)
        {
            _connectionString = connectionString;
            Events = new Collection<IEvent>();
        }

        internal IDbConnection Connection
        {
            get
            {
                return _connection ?? new MySqlConnection(_connectionString);
            }
        }

        public IAuditService AuditService { get; set; }

        public void QueueEvent(IEvent eventToFire)
        {
            Events.Add(eventToFire);
        }

        public void FireEvents()
        {
            foreach (var e in Events ?? new IEvent[] {})
            {
                e.Fire();
            }
        }

        public IEnumerable<EventDescription> Fetch()
        {
            using (var cn = Connection)
            {
                    cn.Open();
                    var sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events`;";

                    var events = cn.Query<EventDescription>(sql);

                    sql = "SELECT `ID`, `EventID`, `Tag`, `Description` FROM `EventReplacementTags`;";

                    var eventTags = cn.Query<EventTag>(sql);

                    if (!eventTags.IsEmpty())
                    {
                        var eventTagsDict = eventTags.GroupBy(n => n.EventID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var ev in events)
                        {
                            List<EventTag> evTagList;

                            if (eventTagsDict.TryGetValue(ev.ID, out evTagList))
                                ev.Tags = evTagList;
                        }
                    }

                    return events;
            }
        }

        public EventDescription Fetch(int Id)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events` WHERE `Id`=@id;";

                var ev = cn.Query<EventDescription>(sql, new {id = Id}).SingleOrDefault();

                sql = "SELECT `ID`, `EventID`, `Tag`, `Description` FROM `EventReplacementTags` WHERE `EventID`=@EventID;";

                ev.Tags = cn.Query<EventTag>(sql, new { EventID = Id });

                return ev;
            }
        }

        public IEnumerable<EventDescription> Fetch(IEnumerable<int> Ids)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events` WHERE `Id` IN @ids;";
                var events = cn.Query<EventDescription>(sql, new {ids = Ids});

                sql = "SELECT `ID`, `EventID`, `Tag`, `Description` FROM `EventReplacementTags` WHERE `EventID` IN @EventIds;";

                var eventTags = cn.Query<EventTag>(sql, new { EventIds = Ids });

                if (!eventTags.IsEmpty())
                {
                    var eventTagsDict = eventTags.GroupBy(n => n.EventID).ToDictionary(k => k.Key, v => v.ToList());

                    foreach (var ev in events)
                    {
                        List<EventTag> evTagList;

                        if (eventTagsDict.TryGetValue(ev.ID, out evTagList))
                            ev.Tags = evTagList;
                    }
                }

                return events;
            }
        }

        public IEnumerable<EventDescription> FetchForApplication(int applicationId)
        {
            using (var cn = Connection)
            {
                var sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events` WHERE `ApplicationId`=@applicationId;";

                var events = cn.Query<EventDescription>(sql, new { applicationId = applicationId });

                if (!events.IsEmpty())
                {
                    sql = "SELECT `ID`, `EventID`, `Tag`, `Description` FROM `EventReplacementTags` WHERE `EventID` IN @EventIds;";

                    var eventTags = cn.Query<EventTag>(sql, new {EventIds = events.Select(n => n.ID)});

                    if (!eventTags.IsEmpty())
                    {
                        var eventTagsDict = eventTags.GroupBy(n => n.EventID).ToDictionary(k => k.Key, v => v.ToList());

                        foreach (var ev in events)
                        {
                            List<EventTag> evTagList;

                            if (eventTagsDict.TryGetValue(ev.ID, out evTagList))
                                ev.Tags = evTagList;
                        }
                    }
                }

                return events;
            }
        }

        public EventDescription New()
        {
            return new EventDescription { Tags = new EventTag[] {}};
        }

        internal bool eventDescriptionObjHasChanged(EventDescription oldEv, EventDescription newEv)
        {
            if (oldEv == null)
                return false;

            if (oldEv.Enabled != newEv.Enabled ||
                oldEv.Description != newEv.Description ||
                oldEv.Name != newEv.Name ||
                oldEv.NiceName != newEv.NiceName ||
                oldEv.System != newEv.System)
                return true;



            if (!newEv.Tags.IsEmpty() && newEv.Tags.Any(n => n.ID == 0))
                return true;


            //TODO: This logic needs some work, it needs to first look for new, and return true, or look for the same ids then compare fields of objects with the same id
            HashSet<string> oldTagsHash = null;
            if (!oldEv.Tags.IsEmpty())
                oldTagsHash = new HashSet<string>(oldEv.Tags.Select(n => n.Tag));


            HashSet<string> newTagsHash = null;
            if (!newEv.Tags.IsEmpty())
                newTagsHash = new HashSet<string>(newEv.Tags.Select(n => n.Tag));


            if (!oldTagsHash.IsEmpty() && !newTagsHash.IsEmpty())
                return !oldTagsHash.Any(newTagsHash.Contains);

            if (oldTagsHash.IsEmpty() && !newTagsHash.IsEmpty())
                return true;

            if (!oldTagsHash.IsEmpty() && newTagsHash.IsEmpty())
                return true;
           // if()

            return false;
        }

        public bool BulkProcessApplicationEvents(IEnumerable<EventDescription> newAppEvents, int applicationId)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    AuditService.DbConnect(cn);

                    cn.Open();
                    var success = true;

                    var sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events` WHERE `ApplicationID`=@applicationId;";

                    var oldAppEvents = cn.Query<EventDescription>(sql, new { applicationId = applicationId });

                    if (!oldAppEvents.IsEmpty())
                    {
                        sql = "SELECT `ID`, `EventID`, `Tag`, `Description` FROM `EventReplacementTags` WHERE `EventID` IN @EventIds;";

                        var eventTags = cn.Query<EventTag>(sql, new {EventIds = oldAppEvents.Select(n => n.ID)});

                        if (!eventTags.IsEmpty())
                        {
                            var eventTagsDict = eventTags.GroupBy(n => n.EventID)
                                .ToDictionary(k => k.Key, v => v.ToList());

                            foreach (var ev in oldAppEvents)
                            {
                                List<EventTag> evTagList;

                                if (eventTagsDict.TryGetValue(ev.ID, out evTagList))
                                    ev.Tags = evTagList;
                            }
                        }
                    }

                    if (newAppEvents.IsEmpty() && !oldAppEvents.IsEmpty())
                    {//All events have been removed, so just delete all
                        sql = "DELETE FROM `Events` WHERE `ApplicationId`=@applicationId;";
                        success &= cn.Execute(sql, new {applicationId = applicationId}) > 0;
                        sql = "DELETE FROM `EventReplacementTags` WHERE `EventID` in @EventIDs;";

                        success &= cn.Execute(sql, new {EventIDs = oldAppEvents.Select(n => n.ID)}) > 0;
                        if (success)
                        {
                            foreach (var ev in oldAppEvents)
                            {
                                AuditService.ObjectAuditLog(ActionType.Delete, n => n.ID, ev);
                            }
                        }

                    }
                    else
                    {
                        if (!newAppEvents.IsEmpty() && oldAppEvents.IsEmpty())
                        {//All events are new, so just do a create
                            sql = "INSERT INTO `Events` (`ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System`) VALUES (@ApplicationId, @Name, @NiceName, @Description, @Enabled, @System);";

                            var newInsertEvents = newAppEvents.Select(n =>
                            {
                                n.ApplicationId = applicationId;
                                return n;
                            }).ToList();

                            ulong newId = 0;
                            foreach (var create in newInsertEvents)
                            {
                                success &= cn.Execute(sql, create) > 0;

                                newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                                create.ID = Convert.ToInt32(newId);

                                if (!create.Tags.IsEmpty())
                                {
                                    create.Tags = ProcessEventTags(cn, create.Tags, ref success, create.ID);
                                }
                            }

                            if (success)
                            {
                                foreach (var ev in newInsertEvents)
                                {
                                    AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, ev);
                                }
                            }
                        }
                        else if (!newAppEvents.IsEmpty() && !oldAppEvents.IsEmpty())
                        {//sioome events exist, so we need to sort out what is new, what needs updating, and what should be deleted
                            var toDelete = oldAppEvents.Where(n => !newAppEvents.Any(m => n.ID == m.ID && m.ID > 0));

                            var toUpdate = newAppEvents.Where(n => oldAppEvents.Any(m => n.ID == m.ID));

                            var toCreate = newAppEvents.Where(n => n.ID == 0);

                            var toCreateFinal = toCreate.Select(n =>
                            {
                                n.ApplicationId = applicationId;
                                return n;
                            }).ToList();

                            if (!toDelete.IsEmpty())
                            {
                                sql = "DELETE FROM `Events` WHERE `id`=@id;";

                                success &= cn.Execute(sql, toDelete) > 0;

                                sql = "DELETE FROM `EventReplacementTags` WHERE `EventID` in @EventIDs;";

                                success &= cn.Execute(sql, new {EventIDs = toDelete.Select(n => n.ID)}) > 0;

                                if (success)
                                {
                                    foreach (var ev in toDelete)
                                    {
                                        AuditService.ObjectAuditLog(ActionType.Delete, n => n.ID, ev);
                                    }
                                }
                            }

                            if (!toUpdate.IsEmpty())
                            {
                                var toUpdateOld = oldAppEvents.Where(n => newAppEvents.Any(m => n.ID == m.ID));

                                var updateList = new Collection<EventDescription>();

                                foreach (var ev in toUpdate.Where(ev => eventDescriptionObjHasChanged(toUpdateOld.Single(n => n.ID == ev.ID), ev)))
                                {
                                    updateList.Add(ev);
                                }

                                if (updateList.Count > 0)
                                {
                                    sql =
                                        "UPDATE `Events` SET `ApplicationId`=@ApplicationId, `Name`=@Name, `NiceName`=@NiceName, `Description`=@Description, `Enabled`=@Enabled, `System`=@System WHERE `ID`=@ID;";
                                    success &= cn.Execute(sql, updateList) > 0;

                                    if (success)
                                    {

                                        foreach (var update in updateList)
                                        {
                                            update.Tags = ProcessEventTags(cn, update.Tags, ref success, update.ID);
                                        }

                                        foreach (var ev in updateList)
                                        {
                                            AuditService.ObjectAuditLog(ActionType.Update, n => n.ID, ev);
                                        }
                                    }
                                }
                            }

                            if (!toCreate.IsEmpty())
                            {
                                sql = "INSERT INTO `Events` (`ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System`) VALUES (@ApplicationId, @Name, @NiceName, @Description, @Enabled, @System);";
                                ulong newId = 0;

                                foreach (var create in toCreate)
                                {
                                    success &= cn.Execute(sql, create) > 0;

                                    newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                                    create.ID = Convert.ToInt32(newId);
                                }

                                if (success)
                                {
                                    foreach (var ev in toCreateFinal)
                                    {
                                        ev.Tags = ProcessEventTags(cn, ev.Tags, ref success, ev.ID);

                                        AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, ev);
                                    }
                                }
                                //  _connection.Execute()
                            }
                        }
                    }

                    if (!success)
                        transaction.Dispose();
                    else
                        transaction.Complete();

                    return success;
                }
            }
        }

        private IEnumerable<EventTag> ProcessEventTags(IDbConnection cn, IEnumerable<EventTag> newTags, ref bool success, int eventID)
        {
            var updatedTags = new List<EventTag>();

            var sql = "SELECT `ID`, `EventID`, `Tag`, `Description` FROM `EventReplacementTags` WHERE `EventID`=@EventID;";

            var oldTags = cn.Query<EventTag>(sql, new {EventID = eventID});

            if (newTags.IsEmpty() && !oldTags.IsEmpty())
            {//All tags have been removed, so just delete all
                sql = "DELETE FROM `EventReplacementTags` WHERE `EventID`=@EventID;";
                success &= cn.Execute(sql, new { EventID = eventID }) > 0;
            }
            else
            {
                if (!newTags.IsEmpty() && oldTags.IsEmpty())
                {
                    sql = "INSERT INTO `EventReplacementTags` (`ID`, `EventID`, `Tag`, `Description`) VALUES (@ID, @EventID, @Tag, @Description)";

                    var insertNewTags = newTags.Select(n =>
                    {
                        n.EventID = eventID;
                        return n;
                    });

                    foreach (var create in insertNewTags)
                    {
                        success &= cn.Execute(sql, create) > 0;

                        var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                        create.ID = Convert.ToInt32(newId);
                    }

                    return insertNewTags;
                }

                if (!newTags.IsEmpty() && !oldTags.IsEmpty())
                {
                    var toDelete = oldTags.Where(n => !newTags.Any(m => n.ID == m.ID && m.ID > 0));

                    var toUpdate = newTags.Where(n => oldTags.Any(m => n.ID == m.ID));

                    var toCreate = newTags.Where(n => n.ID == 0);
                    
                    var toCreateFinal = toCreate.Select(n =>
                    {
                        n.EventID = eventID;
                        return n;
                    }).ToList();

                    if (!toDelete.IsEmpty())
                    {
                        sql = "DELETE FROM `EventReplacementTags` WHERE `id`=@id;";

                        success &= cn.Execute(sql, toDelete) > 0;
                    }

                    if (!toUpdate.IsEmpty())
                    {
                        /*
                            HashSet<int> oldTagsHash = null;
                            if (!oldEv.Tags.IsEmpty())
                                oldTagsHash = new HashSet<int>(oldEv.Tags.Select(n => n.ID));


                            HashSet<int> newTagsHash = null;
                            if (!newEv.Tags.IsEmpty())
                                newTagsHash = new HashSet<int>(newEv.Tags.Select(n => n.ID));


                            if (!oldTagsHash.IsEmpty() && !newTagsHash.IsEmpty())
                                return oldTagsHash.Any(newTagsHash.Contains);*/
                        sql = "UPDATE `EventReplacementTags` SET `Tag`=@tag, `Description`=@description WHERE `ID`=@id";

                        success &= cn.Execute(sql, toUpdate) > 0;

                        updatedTags.AddRange(toUpdate);
                        //TODO: WE NEED TO CHECK IF TAG LIST HAS CHANGED INDEPENDENT FROM EVENT? HOW DO WE NOTIFIY EVENT IF EVENT IS THE SAME BUT TAGS HAVE CHANGED??
                    }

                    if (!toCreateFinal.IsEmpty())
                    {
                        sql = "INSERT INTO `EventReplacementTags` (`ID`, `EventID`, `Tag`, `Description`) VALUES (@ID, @EventID, @Tag, @Description)";

                        var insertNewTags = toCreateFinal.Select(n =>
                        {
                            n.EventID = eventID;
                            return n;
                        });

                        foreach (var create in insertNewTags)
                        {
                            success &= cn.Execute(sql, create) > 0;

                            var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                            create.ID = Convert.ToInt32(newId);
                        }


                        updatedTags.AddRange(insertNewTags);
                    }

                    return updatedTags;
                }
            }

            return null;
        }

        public bool Create(EventDescription eventDescription, out int id)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;
                    id = -1;

                    var sql = "INSERT INTO `Events` (`ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System`) VALUES (@ApplicationId, @Name, @NiceName, @Description, @Enabled, @System);";
                    success &= cn.Execute(sql, eventDescription) > 0;

                    var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    id = Convert.ToInt32(newId);

                    sql = ""; //TODO: Insert event Tags here!

                    transaction.Complete();

                    return success | newId > 0;
                }
            }

        }

        public bool Update(EventDescription eventDescription)
        {
            using (var cn = Connection)
            {
                const string sql = "UPDATE `Events` SET `ApplicationId`=@ApplicationId, `Name`=@Name, `NiceName`=@NiceName, `Description`=@Description, `Enabled`=@Enabled, `System`=@System WHERE `ID`=@id;";

                return cn.Execute(sql, new { ApplicationId = eventDescription.ApplicationId, Name = eventDescription.Name, NiceName = eventDescription.NiceName, Description=eventDescription.Description, Enabled=eventDescription.Enabled, System=eventDescription.System, id = eventDescription.ID }) > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var cn = Connection)
            {
                const string sql = "DELETE FROM `Events` WHERE `ID`=@id;";

                return cn.Execute(sql, new { id = id }) > 0;
            }
        }

        public bool Put(EventDescription eventDescription)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;

                    var sql = "DELETE FROM `Events` WHERE `ID`=@id;";
                    success &= cn.Execute(sql, new { id = eventDescription.ID }) > 0;

                    sql = "INSERT INTO `Events` (`ID`,`ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System`) VALUES (@ID, @ApplicationId, @Name, @NiceName, @Description, @Enabled, @System);";
                    success &= cn.Execute(sql, eventDescription) > 0;


                    if (success) transaction.Complete(); else transaction.Dispose();

                    return success;
                }
            }
        }
         
        public bool AssignToEvent<T>(Func<T, int> idField, T Object, int IdEvent) where T : class
        {
            using (var cn = Connection)
            {//TODO: Have created and last updated?
                const string sql = "INSERT INTO `ObjectAssignedToEvent` (`EventID`, `ObjectID`, `Type`, `Created`, `CreatedBy`) VALUES (@EventID, @ObjectID, @Type, @Created, @CreatedBy)";

                return cn.Execute(sql, new { EventID = IdEvent, ObjectID = idField, Type = typeof(T), Created = DateTime.Now, CreatedBy = "Unknown" }) > 0;
            }
        }

        public bool AssignToEvent(EventObject eventObject)
        {
            using (var cn = Connection)
            {//TODO: Have created and last updated?
                const string sql = "INSERT INTO `ObjectAssignedToEvent` (`EventID`, `ObjectID`, `Type`, `Created`, `CreatedBy`) VALUES (@EventID, @ObjectID, @Type, @Created, @CreatedBy)";

                return cn.Execute(sql, new { EventID = eventObject.EventId, ObjectID = eventObject.ObjectId, Type = eventObject.Type, Created = DateTime.Now, CreatedBy = "Unknown" }) > 0;
            }
        }

        public IEnumerable<int> FetchEventItems(Type objectType, int idEvent)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `ObjectID` FROM `ObjectAssignedToEvent` WHERE `Type`=@type AND `EventID`=@EventID;";

                return cn.Query<int>(sql, new {Type = objectType.ToString(), EventID = idEvent});
            }
        }

        public IEnumerable<int> FetchEventItems(Type objectType, string eventName)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `ObjectID` FROM `ObjectAssignedToEvent` a INNER JOIN `Events` b ON a.`EventID`=b.`ID` WHERE a.`Type`=@type AND b.`Name`=@Name;";

                return cn.Query<int>(sql, new { Type = objectType.ToString(), Name = eventName });
            }
        }

        public IEnumerable<int> FetchEventItems(string eventName)
        {
            throw new NotImplementedException();
        }

        public bool RollBack(AuditRecord record)
        {
            var rollBackApplication = record.Object.FromJson<EventDescription>();
            var success = Put(rollBackApplication);

            if (success)
            {
                AuditService.ObjectAuditLog(ActionType.RollBack, n => n.ID, rollBackApplication, record);
            //    EventManagerService.QueueEvent(new ApplicationRolledBackEvent(rollBackApplication));
            }

            return success;
        }
    }
}
