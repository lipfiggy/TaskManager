using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Threading;

namespace TaskManagerMVC.Controllers
{
    public class WebApiAuthenticationHttpClientHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFlurlClient _flurlClient;
        public WebApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor,
            IFlurlClientFactory flurlClientFactory)
        {
            _flurlClient = flurlClientFactory.Get(Constants.WebApiLink);
            _httpContextAccessor = httpContextAccessor;
            var token = httpContextAccessor.HttpContext.Request.Cookies[Constants.UserJWT];
            if(!string.IsNullOrWhiteSpace(token))
            {
                _flurlClient.WithOAuthBearerToken(token);
            }
        }


    }
}
