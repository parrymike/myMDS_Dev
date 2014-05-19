﻿using System;
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
                const string sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events`;";

                return cn.Query<EventDescription>(sql);
            }
        }

        public EventDescription Fetch(int Id)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events` WHERE `Id`=@id;";

                return cn.Query<EventDescription>(sql, new {id = Id}).SingleOrDefault();
            }
        }

        public IEnumerable<EventDescription> Fetch(IEnumerable<int> Ids)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events` WHERE `Id` IN @ids;";

                return cn.Query<EventDescription>(sql, new {ids = Ids});
            }
        }

        public IEnumerable<EventDescription> FetchForApplication(int applicationId)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System` FROM `Events` WHERE `ApplicationId`=@applicationId;";

                return cn.Query<EventDescription>(sql, new { applicationId = applicationId });
            }
        }

        public EventDescription New()
        {
            return new EventDescription();
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

                    if (newAppEvents.IsEmpty() && !oldAppEvents.IsEmpty())
                    {
                        sql = "DELETE FROM `Events` WHERE `ApplicationId`=@applicationId;";
                        success &= cn.Execute(sql, new {applicationId = applicationId}) > 0;

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
                        {
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
                        {
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

        public bool Create(EventDescription eventDescription, out int id)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();

                    var success = true;
                    id = -1;

                    const string sql = "INSERT INTO `Events` (`ApplicationId`, `Name`, `NiceName`, `Description`, `Enabled`, `System`) VALUES (@ApplicationId, @Name, @NiceName, @Description, @Enabled, @System);";
                    success &= cn.Execute(sql, eventDescription) > 0;

                    var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    id = Convert.ToInt32(newId);

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