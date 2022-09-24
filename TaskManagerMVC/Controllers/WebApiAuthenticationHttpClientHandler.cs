using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Threading;

namespace TaskManagerMVC.Controllers
{
    public class WebApiAuthenticationHttpClientHandler:HttpClientHandler
    {
        IHttpContextAccessor _accessor;
        public WebApiAuthenticationHttpClientHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _accessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
