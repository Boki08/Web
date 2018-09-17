using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentApp.Hubs
{
    [HubName("notifications")]
    //[Authorize(Roles = "Admin")]
    public class NotificationsHub : Hub
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
       
        public static void Hello()
        {
            hubContext.Clients.All.hello();
        }
        

        public static void NotifyAdmin(string message)
        {
            hubContext.Clients.All.notify(message);
        }
    }
}