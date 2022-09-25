using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http.Headers;
using TaskManagerModels;

namespace TaskManagerMVC.Controllers
{
    public class UserLoginController : Controller
    {
        IConfiguration _config;

        public UserLoginController(IConfiguration configuration)
        {
            _config = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser([Bind("Email,Password")] UserLoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                using(HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_config[Constants.WebApiLink]);
                    var responseTask = await client.PostAsJsonAsync<UserLoginModel>("login", loginModel);

                    if (responseTask.IsSuccessStatusCode)
                    {
                        var token = responseTask.Content.ReadAsStringAsync().Result;
                        Response.Cookies.Append(Constants.UserJWT, token, new CookieOptions { HttpOnly = true, 
                                                                                     SameSite = SameSiteMode.Strict });
                        //remember token
                        //call groups get method
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Error occured");
                    }

                }
                return RedirectToAction("Index", "Groups");
            }
            return View();
        }
    }
}
