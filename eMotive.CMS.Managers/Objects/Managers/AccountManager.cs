using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using eMotive.CMS.Extensions;
using eMotive.CMS.Managers.Interfaces;
using eMotive.CMS.Models.Objects.Account;
using eMotive.CMS.Models.Objects.Users;
using eMotive.CMS.Repositories;
using eMotive.CMS.Repositories.Interfaces;
using eMotive.CMS.Services.Interfaces;
using Ninject;

namespace eMotive.CMS.Managers.Objects.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly IUserRepository userRepository;

        private readonly string connectionString;

        public AccountManager(string _connectionString, IUserRepository _userRepository)
        {
            connectionString = _connectionString;
            userRepository = _userRepository;

          //  AutoMapperManagerConfiguration.Configure();
        }

      //  [Inject]
      //  public INotificationService notificationService { get; set; }
        [Inject]
        public IMessageBusService MessageBusService { get; set; }
        [Inject]
        public IConfigurationService configurationService { get; set; }

        public bool ValidateUser(string _username, string _password)
        {
            var maxAttempts = configurationService.MaxLoginAttempts();
            if (LoginAttemptCount(_username) >= maxAttempts)
            {
                MessageBusService.AddIssue("Your account has been locked for 15 minutes.");
                return false;
            }

            var user = userRepository.Fetch(_username, FetchByUserField.Username);
            if (user == null)
            {
                MessageBusService.AddIssue("Your username or password was not recognised.");
                LoginAttempt(_username);
                MessageBusService.AddIssue(string.Format("Login attempt {0} of {1}.", LoginAttemptCount(_username), maxAttempts));

                return false;
            }

            var salt = userRepository.GetSalt(_username);

            if (string.IsNullOrEmpty(salt))
            {
               // MessageBusService.Log(string.Format("Account Manager: An error occured while fetching the salt for user '{0}'", _username));
                MessageBusService.AddError("An error occurred when trying to log you in. An administrator has been notified of this issue.");
                return false;
            }

            var password = EncryptPassword(salt, _password);

            var success = userRepository.ValidateUser(_username, password);

            if (success)
                return true;

            MessageBusService.AddIssue("The entered username and password were not recognised.");
            LoginAttempt(_username);
            MessageBusService.AddIssue(string.Format("Login attempt {0} of {1}.", LoginAttemptCount(_username), maxAttempts));
            return false;
        }

        public bool CreateNewAccountPassword(User _user)
        {
            string salt;
            string plainPassword;
            var password = GeneratePassword(out salt, out plainPassword);
            var success = userRepository.SavePassword(_user.ID, salt, password);

            if (success)
            {
                var replacements = new Dictionary<string, string>(4)
                    {
                        {"#forename#", _user.Forename},
                        {"#surname#", _user.Surname},
                        {"#username#", _user.Username},
                        {"#password#", plainPassword},
                      //  {"#sitename#", configurationService.SiteName()},
                       // {"#siteurl#", configurationService.SiteURL()},
                        {"#accounttype#", _user.Roles.HasContent() ? _user.Roles.First().Name : string.Empty},
                    };

                const string key = "AdminCreateAccount";

             /*   if (notificationService.SendMail(key, _user.EventDescription, replacements))
                {
                    notificationService.SendEmailLog(key, _user.Username);
                    return true;
                }*/

                return true;
            }
         //   MessageBusService.Log(string.Format("Account Manager: An error occured while trying to email the account details to  '{0}'", _user.Username));
            MessageBusService.AddError(string.Format("An error occured while trying to email the account details to '{0}'.", _user.Username));
            return false;
        }

        public bool ResendAccountCreationEmail(string _username)
        {
            var user = userRepository.Fetch(_username, FetchByUserField.Username);

            if (user == null)
                return false;

            string salt;
            string plainPassword;
            var password = GeneratePassword(out salt, out plainPassword);
            var success = userRepository.SavePassword(user.ID, salt, password);

            if (success)
            {
                var replacements = new Dictionary<string, string>(4)
                    {
                        {"#forename#", user.Forename},
                        {"#surname#", user.Surname},
                        {"#username#", user.Username},
                        {"#password#", plainPassword},
                 //       {"#sitename#", configurationService.SiteName()},
                  //      {"#siteurl#", configurationService.SiteURL()}
                    };

                const string key = "AdminCreateAccount";

             /*   if (notificationService.SendMail(key, user.EventDescription, replacements))
                {
                    notificationService.SendEmailLog(key, user.Username);
                    return true;
                }*/
            }
          //  MessageBusService.Log(string.Format("Account Manager: An error occured while trying to email the account details to  '{0}'", user.Username));
            MessageBusService.AddError(string.Format("An error occured while trying to email the account details to '{0}'.", user.Username));
            return false;
        }

        /// <summary>
        /// Generates a new password and emails it to a user
        /// </summary>
        /// <param name="_email">An email address</param>
        /// <returns>A bool indicating if the operation was a success</returns>
        public bool ResendPassword(string _email)
        {
            var user = userRepository.Fetch(_email, FetchByUserField.Email);
            if (user == null)
            {
                MessageBusService.AddIssue("Your email address was not recognised.");
                return false;
            }

            string salt;
            string plainPassword;
            var password = GeneratePassword(out salt, out plainPassword);
            var success = userRepository.SavePassword(user.ID, salt, password);

            if (success)
            {
                var replacements = new Dictionary<string, string>(4)
                    {
                        {"#forename#", user.Forename},
                        {"#surname#", user.Surname},
                        {"#username#", user.Username},
                        {"#ip#", configurationService.GetClientIpAddress()},
                        {"#password#", plainPassword},
                        {"#date#", DateTime.Now.ToString("dddd d MMMM yyyy")}//,
                      //  {"#sitename#", configurationService.SiteName()},
                       // {"#siteurl#", configurationService.SiteURL()}
                    };

                const string key = "UserResetPassword";

             /*   if (notificationService.SendMail(key, _email, replacements))
                {
                    notificationService.SendEmailLog(key, user.Username);
                    return true;
                }*/
            }

          //  MessageBusService.Log(string.Format("Account Manager: An error occured while trying to resend a password to  '{0}'", user.Username));
            MessageBusService.AddError("An error occured while trying to generate a new password.");
            return false;
        }

        public bool ResendUsername(string _email)
        {
            var user = userRepository.Fetch(_email, FetchByUserField.Email);
            if (user == null)
            {
                MessageBusService.AddIssue("Your email address was not recognised.");
                return false;
            }
            var replacements = new Dictionary<string, string>(4)
                {
                    {"#forename#", user.Forename},
                    {"#surname#", user.Surname},
                    {"#username#", user.Username},
                    {"#ip#", configurationService.GetClientIpAddress()},
                    {"#date#", DateTime.Now.ToString("dddd d MMMM yyyy")}//,
                 //   {"#sitename#", configurationService.SiteName()},
                  //  {"#siteurl#", configurationService.SiteURL()}
                };

            const string key = "UserRequestUsername";

           /* if (notificationService.SendMail(key, _email, replacements))
            {
                notificationService.SendEmailLog(key, user.Username);
                return true;
            }*/

          //  MessageBusService.Log(string.Format("Account Manager: An error occured while trying to resend username to  '{0}'", _email));
            MessageBusService.AddError("An error occured while trying to email a username reminder.");
            return false;
        }


        public bool ChangePassword(ChangePassword _changePassword)
        {
            var user = userRepository.Fetch(_changePassword.Username, FetchByUserField.Username);
            if (user == null)
            {
//MessageBusService.Log(string.Format("Account Manager: An error occurred while trying to change a users password. The username '{0}' did not belong to a user account.", _changePassword.Username));
                MessageBusService.AddIssue("An error occurred while trying to change your password. Your password has not been changed.");
                return false;
            }

            var salt = userRepository.GetSalt(_changePassword.Username);

            if (string.IsNullOrEmpty(salt))
            {
              //  MessageBusService.Log(string.Format("Account Manager: An error occured while fetching the salt for user '{0}'", _changePassword.Username));
                MessageBusService.AddError("An error occurred when trying to log you in. An administrator has been notified of this issue.");
                return false;
            }

            var password = EncryptPassword(salt, _changePassword.CurrentPassword);

            if (!userRepository.ValidateUser(_changePassword.Username, password))
            {
                MessageBusService.AddIssue("Your current password was entered incorrectly.");

                return false;
            }

            var encryptedPassword = ChangePassword(out salt, _changePassword.NewPassword);

            if (userRepository.SavePassword(user.ID, salt, encryptedPassword))
                return true;

            MessageBusService.AddIssue("An error occurred while trying to change your password. Your password has not been changed.");

            return false;
        }


        private string GeneratePassword(out string _salt, out string _plainPassword)
        {
            var plainPassword = _plainPassword = GenerateAuthCode().Substring(0, 8);

            _salt = GenerateAuthCode();
            return EncryptPassword(_salt, plainPassword);
        }

        private string ChangePassword(out string _salt, string _newPassword)
        {
            _salt = GenerateAuthCode();
            return EncryptPassword(_salt, _newPassword);
        }

        private string EncryptPassword(string _salt, string _password)
        {
            var tempPassword = string.Concat(_password, _salt);

            MD5 md5 = new MD5CryptoServiceProvider();
            var originalBytes = Encoding.Default.GetBytes(tempPassword);
            var encodedBytes = md5.ComputeHash(originalBytes);

            return Regex.Replace(BitConverter.ToString(encodedBytes), "-", "");
        }

        private string GenerateAuthCode()
        {
            var guidResult = Guid.NewGuid().ToString();

            guidResult = guidResult.Replace("-", string.Empty);

            return guidResult.Substring(0, 12);
        }

        //todo: have a accountRepository and have all db code there?
        private void LoginAttempt(string _username)
        {
            var ipAddress = configurationService.GetClientIpAddress();

            if (string.IsNullOrEmpty(ipAddress))
            {
              //  MessageBusService.Log(string.Format("AccountManager: Could not resolve ip address for user '{0}' on failed login attempt.", _username));
                return;
            }
/*
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var sql = "INSERT INTO `loginattempts` (`IP`,`occurred`) VALUES (@ip, @occurred);";
                connection.Execute(sql, new { ip = ipAddress, occurred = DateTime.Now });
            }*/
        }

        private int LoginAttemptCount(string _username)
        {
        /*    var ipAddress = configurationService.GetClientIpAddress();
            var lockoutTime = configurationService.LockoutTimeInMinutes();

            if (string.IsNullOrEmpty(ipAddress))
            {
                MessageBusService.Log(string.Format("AccountManager: Could not resolve ip address for user '{0}' on failed login attempt.", _username));
                return -1;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = "SELECT CAST(COUNT(*)AS UNSIGNED INTEGER) FROM `loginattempts` WHERE `IP`=@ip AND `Occurred` > @occurred;";
                var id = connection.Query<ulong>(sql,
                    new
                    {
                        ip = ipAddress,
                        occurred = DateTime.Now.AddMinutes(-lockoutTime)
                    }).SingleOrDefault();

                return Convert.ToInt32(id);
            }
            */

            throw new NotImplementedException();

        }
    }
}
