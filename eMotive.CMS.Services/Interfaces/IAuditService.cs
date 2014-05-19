using System;
using System.Collections.Generic;
using System.Data;
using eMotive.CMS.Services.Objects;
using eMotive.CMS.Services.Objects.Audit;

namespace eMotive.CMS.Services.Interfaces
{
    /// <summary>
    /// A logging service
    /// </summary>
    public interface IAuditService
    {//TODO: could do with user id etc in rep where this is logged. Perhpas in configuration service have a "Request" object populated? THis can contain authd user, page requested etc - perhaps populated in OnRequestBegin
        //TODO: if we do Have a Request Obj, then replace these ids with user objects?
        /// <summary>
        /// Used to log changes to a object (class)
        /// </summary>
        /// <typeparam name="T">The type of object being altered or created</typeparam>
        /// <param name="action">The action being used on the object</param>
        /// <param name="idField">Field which contains the object ID</param>
        /// <param name="Object">The object with alterations</param>
        /// <returns>Bool indicating if the log entry was successfully created</returns>
        bool ObjectAuditLog<T>(ActionType action, Func<T, int> idField, T Object) where T : class; //do we need new and old object? or ill there be a trail ffrom obj creation, through each update etc.
        bool ObjectAuditLog<T>(ActionType action, Func<T, int> idField, T Object, AuditRecord rollBack) where T : class; //do we need new and old object? or ill there be a trail ffrom obj creation, through each update etc.

        IEnumerable<AuditRecord> FetchLog<T>(int id) where T : class;

        void DbConnect(IDbConnection connection);
        /*
        /// <summary>
        /// Logs a URl request
        /// </summary>
        /// <param name="username">The user initiating the request</param>
        /// <param name="action">The action visited</param>
        /// <param name="controller">The controller visited</param>
        /// <returns>Bool indicating if the log entry was successfully created</returns>*/
        // bool LogRequest(string username, string ip, string url, string queryString);//TODO: Should we log querystring and IP too? CW 12/0/2014 06:23
    }
}
