using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Globalcaching.Controllers
{
    public class LiveAPIOAuthController : Controller
    {
        private static string WcfTokenManagerTag = "WcfTokenManager";
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public LiveAPIOAuthController(IWorkContextAccessor workContextAccessor,
            IOrchardServices services,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _workContextAccessor = workContextAccessor;
            _gcEuUserSettingsService = gcEuUserSettingsService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public ActionResult Index(string ReturnUrl)
        {
            _workContextAccessor.GetContext().HttpContext.Session["ReturnUrl"] = ReturnUrl;
            WebConsumer consumer = CreateConsumer();
            var url = Url.Action("LiveAPICallback", "LiveAPIOAuth", new { area = "Globalcaching" });
            url = string.Format("{0}://{1}{2}", _workContextAccessor.GetContext().HttpContext.Request.Url.Scheme, _workContextAccessor.GetContext().HttpContext.Request.Headers["Host"], url);
            UriBuilder callback = new UriBuilder(url); // the groundspeak oauth handler will call back to this page

            callback.Query = null;
            // At this time Groundspeak is not distinguishing permissions for its API. "All" is the only setting, but you must include the "scope" parameter

            string scope = "All";
            var requestParams = new Dictionary<string, string> {
			    { "scope", scope },
		    };

            try
            {
                // send initial request to and receive response from service provider
                // must send consumersecret and consumerkey in this call

                var response = consumer.PrepareRequestUserAuthorization(callback.Uri, requestParams, null);

                // immediately send response back to service provider to request 
                consumer.Channel.Send(response);
            }
            catch
            {
            }

            return null;
        }

        public ActionResult LiveAPICallback()
        {
            var tokenManager = _workContextAccessor.GetContext().HttpContext.Session[WcfTokenManagerTag] as InMemoryTokenManager;
            if (tokenManager != null)
            {
                if (_workContextAccessor.GetContext().HttpContext.Request.QueryString["oauth_verifier"] != null && _workContextAccessor.GetContext().HttpContext.Request.QueryString["oauth_token"] != null)
                {
                    WebConsumer consumer = CreateConsumer();
                    var accessTokenMessage = consumer.ProcessUserAuthorization();
                    if (accessTokenMessage != null && accessTokenMessage.AccessToken != null)
                    {
                        var settings = _gcEuUserSettingsService.GetSettings();
                        if (settings != null)
                        {
                            settings.LiveAPIToken = accessTokenMessage.AccessToken;
                            _workContextAccessor.GetContext().HttpContext.Session[WcfTokenManagerTag] = null;
                            var profile = LiveAPIClient.GetMemberProfile(settings.LiveAPIToken);
                            if (profile != null)
                            {
                                settings.GCComUserID = profile.User.Id;
                                using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
                                {
                                    var usr = db.SingleOrDefault<GCComUser>("where ID=@0", profile.User.Id);
                                    if (usr == null)
                                    {
                                        db.Insert(GCComUser.From(profile.User));
                                    }
                                    else
                                    {
                                        db.Update("GCComUser", "ID", GCComUser.From(profile.User));
                                    }
                                }
                                _gcEuUserSettingsService.UpdateSettings(settings);
                            }
                            Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("Geocaching.com autorisatie is geslaagd."));
                            return new RedirectResult(_workContextAccessor.GetContext().HttpContext.Session["ReturnUrl"] as string);
                        }
                    }
                }
            }

            Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Er is een fout opgetreden bij de autorisatie."));
            return new RedirectResult(_workContextAccessor.GetContext().HttpContext.Session["ReturnUrl"] as string);
        }

        private WebConsumer CreateConsumer()
        {
            MessageReceivingEndpoint oauthRequestTokenEndpoint;
            MessageReceivingEndpoint oauthUserAuthorizationEndpoint;
            MessageReceivingEndpoint oauthAccessTokenEndpoint;

            // use Post Requests and appropriate endpoints
            oauthRequestTokenEndpoint = new MessageReceivingEndpoint(new Uri(ConfigurationManager.AppSettings["gccom_oauth_live"]), HttpDeliveryMethods.PostRequest);
            oauthUserAuthorizationEndpoint = new MessageReceivingEndpoint(new Uri(ConfigurationManager.AppSettings["gccom_oauth_live"]), HttpDeliveryMethods.PostRequest);
            oauthAccessTokenEndpoint = new MessageReceivingEndpoint(new Uri(ConfigurationManager.AppSettings["gccom_oauth_live"]), HttpDeliveryMethods.PostRequest);

            // in memory token manager should not be used in production. an actual database should be used instead to remember a user's tokens
            var tokenManager = _workContextAccessor.GetContext().HttpContext.Session[WcfTokenManagerTag] as InMemoryTokenManager;
            if (tokenManager == null)
            {
                tokenManager = new InMemoryTokenManager(ConfigurationManager.AppSettings["gccom_consumerkey_live"], ConfigurationManager.AppSettings["gccom_consumersecret_live"]);
                _workContextAccessor.GetContext().HttpContext.Session[WcfTokenManagerTag] = tokenManager;
            }

            // set up web consumer
            WebConsumer consumer = new WebConsumer(
                new ServiceProviderDescription
                {
                    RequestTokenEndpoint = oauthRequestTokenEndpoint,
                    UserAuthorizationEndpoint = oauthUserAuthorizationEndpoint,
                    AccessTokenEndpoint = oauthAccessTokenEndpoint,
                    TamperProtectionElements = new DotNetOpenAuth.Messaging.ITamperProtectionChannelBindingElement[] {
					    new HmacSha1SigningBindingElement(),
				    },
                },
                tokenManager);

            return consumer;
        }

    }
}