namespace eMotive.CMS.Services.Objects
{
    /// <summary>
    /// Used by IMessageBusService to define notification type
    /// </summary>
    public enum MessageType { Issue, Error, Log }
    /// <summary>
    /// Used by ILogService ObjectAuditLog to specifiy the action being carried out on the specified object
    /// </summary>
    public enum ActionType { Create, Update, Delete, RollBack }
}
