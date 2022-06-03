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
    public class AuthenticationHelper
    {
        private static string CacheFilePath
        {
            get
            {
                string _cachepath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TokenCache.json";
                if (!SysIO.File.Exists(_cachepath)) SysIO.File.WriteAllText(_cachepath, "{}");
                return _cachepath;
            }
        }
        private static readonly object FileLock = new object();


        private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            lock (FileLock)
            {
                args.TokenCache.DeserializeMsalV3(SysIO.File.ReadAllBytes(CacheFilePath));
            }
        }

        private static void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    // reflect changes in the persistent store
                    SysIO.File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
                }
            }
        }

        public async Task<string> AccessToken()
        {
            AuthenticationConfig config = AuthenticationConfig.ReadFromJsonFile("appsettings.json");


            IPublicClientApplication app = PublicClientApplicationBuilder.Create(config.ClientId)
                .WithRedirectUri(config.RedirectUrl)
                .WithAuthority(config.Authority)
                .Build();
            app.UserTokenCache.SetBeforeAccess(BeforeAccessNotification);
            app.UserTokenCache.SetAfterAccess(AfterAccessNotification);

            AuthenticationResult result = null;
            try
            {
                AcquireTokenSilentParameterBuilder parambld1 = app.AcquireTokenSilent(config.Scopes, config.LoginUser);
                result = await parambld1.ExecuteAsync();
            }
            catch (Exception ex)
            {
                AcquireTokenInteractiveParameterBuilder parambld = app.AcquireTokenInteractive(config.Scopes);
                result = await parambld.WithPrompt(Microsoft.Identity.Client.Prompt.ForceLogin).ExecuteAsync();
            }

            return (result != null) ? result.AccessToken : null;

        }
    }
}
