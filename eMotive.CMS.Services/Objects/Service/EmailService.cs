using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Transactions;
using Dapper;
using eMotive.CMS.Extensions;
using eMotive.CMS.Models.Objects.Search;
using eMotive.CMS.Search.Interfaces;
using eMotive.CMS.Search.Objects;
using eMotive.CMS.Services.Interfaces;
using eMotive.CMS.Services.Objects.Audit;
using eMotive.CMS.Services.Objects.EmailService;
using MySql.Data.MySqlClient;

namespace eMotive.CMS.Services.Objects.Service
{
    public class EmailService : IEmailService
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public EmailService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ISearchManager SearchManager { get; set; }
        public IMessageBusService MessageBusService { get; set; }
        public IAuditService AuditService { get; set; }

        internal IDbConnection Connection
        {
            get
            {
                return _connection ?? new MySqlConnection(_connectionString);
            }
        }

        public bool Send(Email email, /*IDictionary<string, string> replacements = null,*/ BinaryAttachment[] binaryAttachments = null)
        {
            var client = new SmtpClient();
            var message = new MailMessage();

            message.To.Add(email.To);
            message.Priority = email.Priority;
          //  message.Attachments = email.Attachments;
            message.From = new MailAddress("withersc@bham.ac.uk");
            message.Subject = email.Subject;
            message.Body = email.Body;
            message.IsBodyHtml = email.IsBodyHtml;

            if (email.Enabled)
            {
                client.Send(message);
            }


            client.Dispose();
            message.Dispose();

            return true;
        }

        public bool Send(IEnumerable<Email> emails, BinaryAttachment[] binaryAttachments)
        {
            var client = new SmtpClient();
            var message = new MailMessage();
            foreach (var email in emails)
            {
                

                message.To.Add(email.To);
                message.Priority = email.Priority;
                //  message.Attachments = email.Attachments;
                message.From = new MailAddress("withersc@bham.ac.uk");
                message.Subject = email.Subject;
                message.Body = email.Body;
                message.IsBodyHtml = email.IsBodyHtml;

                if (email.Enabled)
                {
                    client.Send(message);
                }

                message = new MailMessage();
            }

            client.Dispose();
            message.Dispose();

            return true;
        }

        public Email New()
        {
            return new Email { IsBodyHtml = true, Enabled = true };
        }

        public bool Create(Email email, out int id)
        {
            using (var cn = Connection)
            {
                using (var transaction = new TransactionScope())
                {
                    cn.Open();
                    AuditService.DbConnect(cn);

                    var success = true;
                    id = -1;

                    const string sql = "INSERT INTO `Emails` (`To`, `CC`, `BCC`, `Subject`, `Body`, `Priority`, `IsBodyHtml`, `Enabled`) Values (@To, @CC, @BCC, @Subject, @Body, @Priority, @IsBodyHtml, @Enabled);";
                    success &= cn.Execute(sql, new
                    {
                        To = email.To,
                        CC = email.CC,
                        BCC = email.BCC,
                        Subject = email.Subject,
                        Body = email.Body,
                        Priority = email.Priority,
                        Enabled = email.Enabled,
                        IsBodyHtml = email.IsBodyHtml
                    }) > 0;

                    var newId = cn.Query<ulong>("SELECT CAST(LAST_INSERT_ID() AS UNSIGNED INTEGER);").SingleOrDefault();
                    id = email.ID = Convert.ToInt32(newId);

                    if (success)
                        AuditService.ObjectAuditLog(ActionType.Create, n => n.ID, email);

                    transaction.Complete();

                    return success | newId > 0;
                }
            }
        }

        public bool Update(Email email)
        {
            using (var cn = Connection)
            {
                AuditService.DbConnect(cn);
                const string sql = "UPDATE `Emails` SET `To`=@To, `CC`=@CC, `BCC`=@BCC, `Subject`=@Subject, `Body`=@Body, `Priority`=@Priority, `IsBodyHtml`=@IsBodyHtml, `Enabled`=@Enabled WHERE `ID`=@id;";

                var success = cn.Execute(sql, new
                {
                    To = email.To,
                    CC = email.CC,
                    BCC = email.BCC,
                    Subject = email.Subject,
                    Body = email.Body,
                    Priority = email.Priority,
                    Enabled = email.Enabled,
                    IsBodyHtml = email.IsBodyHtml,
                    Id = email.ID
                }) > 0;

                if (success)
                    AuditService.ObjectAuditLog(ActionType.Update, n => n.ID, email);

                return success;
            }
        }

        public bool Delete(int id)
        {//EventDescription EventDescription
            using (var cn = Connection)
            {
                const string sql = "DELETE FROM `Emails` WHERE `ID`=@id;";

                return cn.Execute(sql, new { id = id }) > 0;
            }
        }

        public bool Put(Email email)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Email> Fetch()
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `To`, `CC`, `BCC`, `Subject`, `Body`, `Priority`, `IsBodyHtml`, `Enabled` FROM `Emails`;";

                return cn.Query<Email>(sql);
            }
        }

        public IEnumerable<Email> Fetch(IEnumerable<int> ids)
        {
            using (var cn = Connection)
            {
                const string sql = "SELECT `Id`, `To`, `CC`, `BCC`, `Subject`, `Body`, `Priority`, `IsBodyHtml`, `Enabled` FROM `Emails` WHERE `Id` in @ids;";

                return cn.Query<Email>(sql, new {ids = ids});
            }
        }

        public SearchResult DoSearch(BasicSearch search)
        {
            throw new System.NotImplementedException();
        }

        public void ReindexSearchRecords()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Email> FetchRecordsFromSearch(SearchResult searchResult)
        {
            throw new System.NotImplementedException();
            if (!searchResult.Items.IsEmpty())
            {
                return Fetch(searchResult.Items.Select(n => n.ID));
            }

            return null;
        }

        public bool RollBack(AuditRecord record)
        {
            throw new System.NotImplementedException();
            var rollBackEmail = record.Object.FromJson<Email>();

            var success = Put(rollBackEmail);

          //  if(success)
           //     AuditService.ObjectAuditLog(ActionType.RollBack, n => n.ID, rollBackEmail,)
        }
    }
}
