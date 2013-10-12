using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Thinktecture.IdentityModel.WinRT;
using Thinktecture.Samples;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ImplicitClientWindows8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string _resourceName = "backend";
        TokenCredential _credential;

        //static Uri _baseAddress = new Uri(Constants.WebHostv1BaseAddress);
        static Uri _baseAddress = new Uri(Constants.WebHostv2BaseAddress);

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ClearVault();

            RetrieveStoredToken();
        }

        

        private async void ButtonRequestToken_Click(object sender, RoutedEventArgs e)
        {
            var error = string.Empty;

            try
            {
                var response = await WebAuthentication.DoImplicitFlowAsync(
                    endpoint: new Uri(Constants.AS.OAuth2AuthorizeEndpoint),
                    clientId: Constants.Clients.ImplicitClient,
                    scope: "read");

                TokenVault.StoreToken(_resourceName, response.AccessToken, response.ExpiresIn, response.TokenType);
                RetrieveStoredToken();

            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            if (!string.IsNullOrEmpty(error))
            {
                var dialog = new MessageDialog(error);
                await dialog.ShowAsync();
            }
        }

        private async void ButtonAccessResource_Click(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient { 
                BaseAddress = _baseAddress 
            };

            if (_credential != null)
            {
                client.DefaultRequestHeaders.Authorization =
                           new AuthenticationHeaderValue("Bearer", _credential.AccessToken);
            }

            var response = await client.GetAsync("identity");
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var md = new MessageDialog(response.ReasonPhrase);
                await md.ShowAsync();
                return;
            }
            
            var claims = await response.Content.ReadAsAsync<IEnumerable<ViewClaim>>();

            foreach (var claim in claims)
            {
                ListClaims.Items.Add(string.Format("{0}: {1}", claim.Type, claim.Value));
            }
        }

        private void RetrieveStoredToken()
        {
            TokenCredential credential;
            if (TokenVault.TryGetToken(_resourceName, out credential))
            {
                _credential = credential;
                TextToken.Text = credential.AccessToken;
                TextExpiration.Text = credential.Expires.ToString();
            }
        }

        private void ClearVault()
        {
            try
            {
                var vault = new PasswordVault();
                var cred = vault.Retrieve(_resourceName, "token");
                vault.Remove(cred);
            }
            catch { }
        }

        private void ButtonClearVault_Click(object sender, RoutedEventArgs e)
        {
            ClearVault();
            TextToken.Text = "";
            TextExpiration.Text = "-";
        }
    }

    public class ViewClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
