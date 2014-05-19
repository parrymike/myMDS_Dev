using System;
using System.Web;
using eMotive.CMS.Services.Interfaces;

namespace eMotive.CMS.Services.Objects.Service
{
    public class ConfigurationService : IConfigurationService
    {//TODO: perhaps have a config class which si spit from the ServiceRep. CW 13/03/2014 14:51
        public bool DoLogging()
        {
            return true;
        }

        public bool DoRequestLogging()
        {
            return true;
        }

        public string FetchDefaultReferer()
        {
            return "/Home/Index";
        }

        public string GetClientIpAddress()
        {
            var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }

        public int MaxLoginAttempts()
        {
            const int defaultValue = 5;
            var attemptString = string.Empty;//ConfigurationManager.AppSettings["MaxLoginAttempts"] ?? string.Empty;

            if (string.IsNullOrEmpty(attemptString))
                return defaultValue;

            int attempts;

            return int.TryParse(attemptString, out attempts) ? attempts : defaultValue;
        }

        public int LockoutTimeInMinutes()
        {
            const int defaultValue = 15;
            var lockoutTimeString = String.Empty;// ConfigurationManager.AppSettings["LockoutTimeMinutes"] ?? string.Empty;

            if (string.IsNullOrEmpty(lockoutTimeString))
                return 15;

            int lockoutTime;

            return int.TryParse(lockoutTimeString, out lockoutTime) ? lockoutTime : defaultValue;
        }
    }
}
