using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using Microsoft.Owin.Security;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Services.Objects.Service
{
    public class AuditService : IAuditService
    {//http://stackoverflow.com/questions/2247598/c-sharp-instantiate-class-from-string
        private readonly IAuthenticationManager _authentication;
        private readonly IConfigurationService _configuration;
        private readonly string _connectionString;
        private IDbConnection _connection;
        private readonly bool _doLogging;
        private readonly bool _doRequestLogging;


        public AuditService(IConfigurationService configuration, IAuthenticationManager authentication, string connectionString)
        {
            _authentication = authentication;
            _configuration = configuration;
            _connectionString = connectionString;
            _doLogging = configuration.DoLogging();
            _doRequestLogging = configuration.DoRequestLogging();
        }

        public void DbConnect(IDbConnection connection)
        {
            _connection = connection;
        }

        internal IDbConnection Connection
        {
            get
            {
                //return _connection ?? (_connection = new SqlConnection(_connectionString));
                return new SqlConnection(_connectionString);
            }
        }

        public bool ObjectAuditLog<T>(ActionType action, Func<T, int> idField, T Object) where T : class
        {
            return ObjectAuditLog(action, idField, Object, null);
        }

        public bool ObjectAuditLog<T>(ActionType action, Func<T, int> idField, T Object, AuditRecord rollBack) where T : class
        {
            var username = string.IsNullOrEmpty(_authentication.User.Identity.Name) ? "unknown" : _authentication.User.Identity.Name; //check empty?
            //TODO: can userid put in auth as claim then retrieved here through owin??
            var obj = typeof(T);
            //TODO: get userID from auth Obj??


            using (var connection = Connection)
            {
                const string sql = "INSERT INTO Audit(Date,Username,Action,ObjectID,Type,Object,Details) VALUES (@Date, @Username, @Action, @ObjectID, @Type, @Object, @Details);";

                return connection.Execute(sql,
                    new
                    {
                        Date = DateTime.Now,
                        Username = username,
                        Action = action,
                        ObjectID = idField(Object),
                        Type = obj.FullName,
                        Object = Object.ToJson(),
                        Details = rollBack == null ? string.Empty : string.Format("Rolled Back to {0}: {1}", rollBack.ID, rollBack.Date.ToLongDateString())

                    }) > 0;
            }
        }

        public IEnumerable<AuditRecord> FetchLog<T>(int id) where T : class
        {
            var obj = typeof(T);

            using (var connection = Connection)
            {
                const string sql = "SELECT ID, Date,Username, Action, Object, Details FROM Audit WHERE Type = @Type AND ObjectID = @objectID ORDER BY Date DESC;";

                return connection.Query<AuditRecord>(sql, new {Type = obj.FullName, ObjectID = id});
            }
        }
    }
}
