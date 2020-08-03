using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
    //APIHelper will help handle all of our API interactions
    public class APIHelper : IAPIHelper
    //we created the interface so we can use APIHelper in dependency injection system
    //uncheck ApiClient when you create the interface
    {
        private HttpClient _apiClient;
        private ILoggedInUserModel _loggedInUser;

        public APIHelper(ILoggedInUserModel loggedInUser)
        {
            InitializeClient();
            _loggedInUser = loggedInUser;
        }

        //A read only class, so we can use _apiClient in differnet classes like product
        public HttpClient ApiClient
        {
            
            get
            {
                return _apiClient;
            }

        }
        //every time you call APIHelper will initilaize that client
        //because we will have one HttpClient for the lifespan of the app
        private void InitializeClient()
        {
            //add reference to the System.Configuration  so you can get the the URL from App.config
            //using ConfigurationManager and this will help us because (App.config) can be change at anytime,
            //before compile or after run time in production, also we can create a config file override or merge
            //and get whatever URL value we have in dev, stagging, production , many other areas...
            //we can create app.realease config and replace the URL with whatever value you need
            //we can change the URL value in App.config  and make it variable and use it for different purpose
            //like central development server, central testing server, or central QA server or central production
            //server. All of them can use differnet URL without recompiling the application
            string api = ConfigurationManager.AppSettings["api"];
            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api); //the base address of the URL we get it from App.config
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //Authenticate a method to call out to the API, sending the username and password to get our token info
        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            }); //this data will be send to API endpoint

            using (HttpResponseMessage response = await _apiClient.PostAsync("/Token", data))
            { //the URL will change, now its in alocalhost address, but eventually 
                //will be address in Azure or local web server or something else
                if (response.IsSuccessStatusCode)
                {
                    //to get ReadAsAsync (extension method) -> right click on areference ->  Manage NuGet Packages
                    //then browse (Microsoft.AspNet.WebApi.Client) then download it
                    // create a model AuthenticatedUser to access Token in the API 
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    return result; //if the user provide the correct credentail then we will have the information
                    //and put it in the AuthenticatedUser model and return it
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/User"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                    _loggedInUser.CreatedDate = result.CreatedDate;
                    _loggedInUser.EmailAddress = result.EmailAddress;
                    _loggedInUser.FirstName = result.FirstName;
                    _loggedInUser.Id = result.Id;
                    _loggedInUser.LastName = result.LastName;
                    _loggedInUser.Token = token;

                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
