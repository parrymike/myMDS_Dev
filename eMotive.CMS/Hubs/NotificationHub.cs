using Microsoft.AspNet.SignalR;

namespace eMotive.CMS.Hubs
{
    public class NotificationHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}