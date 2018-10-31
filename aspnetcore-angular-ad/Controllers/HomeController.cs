using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using MyMisWeb.Extensions;
//using MyMisWeb.Models;
using MyMisWeb.Options;
using MyMisWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Diagnostics;
using MyMisWeb.Models;
using MyMisWeb.Data;

namespace MyMisWeb.Controllers
{
    [Authorize]
    public class HomeController : MisBaseController
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly ITokenCacheFactory _tokenCacheFactory;
        private readonly AuthOptions _authOptions;

        public HomeController(MyMisContext context) : base(context)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (Request.Path.Value != "/" && !Request.Path.Value.StartsWith("/home"))
            {
                var path = Request.Path.Value;
                if (path.StartsWith('/'))
                {
                    path = path.Substring(1);
                }
                return Redirect("/home?path=" + path);
            }
            if (!IsCurrentUserActive())
            {
                return View("NotAuthorized");
            }

            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        [HttpGet]
        public IActionResult UserClaims() => View();

        [HttpGet]
        public async Task<IActionResult> MsGraph()
        {
            HttpResponseMessage res = await QueryGraphAsync("/me");

            string rawResponse = await res.Content.ReadAsStringAsync();
            string prettyResponse =
                JsonConvert.SerializeObject(JsonConvert.DeserializeObject(rawResponse), Formatting.Indented);

            var model = new HomeMsGraphModel
            {
                GraphResponse = prettyResponse
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProfilePhoto()
        {
            HttpResponseMessage res = await QueryGraphAsync("/me/photo/$value");

            return File(await res.Content.ReadAsStreamAsync(), "image/jpeg");
        }

        //Normally this stuff would be in another service, not in the controller
        //But for the sake of an example, it is a bit easier
        private async Task<HttpResponseMessage> QueryGraphAsync(string relativeUrl)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/beta" + relativeUrl);

            string accessToken = await GetAccessTokenAsync();
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string authority = _authOptions.Authority;

            var cache = _tokenCacheFactory.CreateForUser(User);

            var authContext = new AuthenticationContext(authority, cache);

            //App's credentials may be needed if access tokens need to be refreshed with a refresh token
            string clientId = _authOptions.ClientId;
            string clientSecret = _authOptions.ClientSecret;
            var credential = new ClientCredential(clientId, clientSecret);
            var userId = User.GetObjectId();

            var result = await authContext.AcquireTokenSilentAsync(
                "https://graph.microsoft.com",
                credential,
                new UserIdentifier(userId, UserIdentifierType.UniqueId));

            return result.AccessToken;
        }
    }
}
