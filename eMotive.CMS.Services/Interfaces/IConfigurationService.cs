namespace eMotive.CMS.Services.Interfaces
{
    /// <summary>
    /// Abstraction for configuration data
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Specifies if Notification Errors should be logged to the database
        /// </summary>
        /// <returns>Bool indicating if the log entry was successfully saved</returns>
        bool DoLogging();
        /// <summary>
        /// Specifies if ebsite requests should be logged
        /// </summary>
        /// <returns>Bool indicating if the log entry was successfully saved</returns>
        bool DoRequestLogging();
        /// <summary>
        /// Returns a default referer
        /// </summary>
        /// <returns>A string representing a referer</returns>
        string FetchDefaultReferer();

        string GetClientIpAddress();

        int MaxLoginAttempts();
        int LockoutTimeInMinutes();
    }
}
