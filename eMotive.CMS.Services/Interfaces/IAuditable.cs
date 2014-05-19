using eMotive.CMS.Services.Objects.Audit;

namespace eMotive.CMS.Services.Interfaces
{
    public interface IAuditable
    {
        bool RollBack(AuditRecord record);
    }
}
