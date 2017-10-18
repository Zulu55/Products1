using Products1.Views;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(LoginFacebookView),
                          typeof(Products1.iOS.Renderers.LoginFacebookRenderer))]

namespace Products1.iOS.Renderers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using Newtonsoft.Json;
    using Xamarin.Auth;
    using Xamarin.Forms.Platform.iOS;

    public class LoginFacebookRenderer : PageRenderer
    {
        bool done = false;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (done)
            {
                return;
            }

            var auth = new OAuth2Authenticator(
                clientId: "1996033220633471",
                scope: "",
                authorizeUrl: new Uri(
                    "https://www.facebook.com/v2.8/dialog/oauth"),
                redirectUrl: new Uri(
                    "http://www.facebook.com/connect/login_success.html"));

            auth.Completed += async (sender, eventArgs) =>
            {
                DismissViewController(true, null);
                App.LoginFacebookFail();

                if (eventArgs.IsAuthenticated)
                {
                    var accessToken =
                        eventArgs.Account.Properties["access_token"].ToString();
                    var profile = await GetFacebookProfileAsync(accessToken);
                    App.LoginFacebookSuccess(profile);
                }
                else
                {
                    App.LoginFacebookSuccess(null);
                }
            };

            done = true;
            PresentViewController(auth.GetUI(), true, null);
        }

        private async Task<FacebookResponse> GetFacebookProfileAsync(
            string accessToken)
        {
            var requestUrl = "https://graph.facebook.com/v2.8/me/?fields=" +
                "name,picture,cover,age_range,devices,email,gender," +
                "is_verified,birthday,languages,work,website,religion," +
                "location,locale,link,first_name,last_name," +
                "hometown&access_token=" + accessToken;
            var httpClient = new HttpClient();
            var userJson = await httpClient.GetStringAsync(requestUrl);
            var facebookResponse =
                JsonConvert.DeserializeObject<FacebookResponse>(userJson);
            return facebookResponse;
        }
    }
}
