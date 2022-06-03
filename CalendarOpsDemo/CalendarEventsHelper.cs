using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Identity.Web;
using System.Security;
using SysIO = System.IO;

namespace CalendarOpsDemo
{
    public class CalendarEventsHelper
    {
        public static async Task<ICalendarEventsCollectionPage> UserCalendarEvents(string useridentity,int numberofdays)
        {
            try
            {
                AuthenticationHelper authhelper = new AuthenticationHelper();
                string acctok = await authhelper.AccessToken();
                GraphServiceClient graphServiceClient =
                        new GraphServiceClient("https://graph.microsoft.com/V1.0/", new DelegateAuthenticationProvider(async (requestMessage) =>
                        {
                        // Add the access token in the Authorization header of the API request.
                        requestMessage.Headers.Authorization =
                                    new AuthenticationHeaderValue("Bearer", acctok);
                        }));

                DateTime dttm = DateTime.Today.AddDays(numberofdays);
                string targetdttm = dttm.ToString("yyyy-MM-ddTHH:mm:ssZ");
                ICalendarEventsCollectionPage calevs = await graphServiceClient.Users[useridentity].Calendar.Events.Request().Filter("start/dateTime le '" + targetdttm+"'").GetAsync();
                return calevs; 
            }
            catch (ServiceException e)
            {
                return null;
            }
        }
    }
}
