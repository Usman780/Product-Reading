using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductReading.DataHub
{
    public class ProgressHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}