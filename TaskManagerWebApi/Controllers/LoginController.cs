using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerModels;
using TaskManagerWebApi.DTO;
using TaskManagerWebApi.Repositories;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly TaskManagerContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public LoginController(IConfiguration config, TaskManagerContext context, IPasswordHasher passwordHasher)
        {
            _config = config;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel userLogin)
        {
            User user = await Authenticate(userLogin);
            if(user != null)
            {
                var token = await GenerateToken(user);
                return Ok(token);
            }
            return BadRequest("User was not found");
        }

        private async Task<TokenDTO> GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);//read about HmacSha256
            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim(ClaimTypes.Email, user.Email),//read about claims
                new Claim("Password", _passwordHasher.GetHashOfAPassword(user.Password)),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(issuer: _config["Jwt:Issuer"],
                                             audience: _config["Jwt:Audience"],
                                             claims: claims,
                                             expires: DateTime.UtcNow.AddDays(1),
                                             signingCredentials: credentials);


            return new TokenDTO { Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }

        private async Task<User> Authenticate(UserLoginModel userLogin)
        {
            var foundUser = _context.Users.FirstOrDefault(user => user.Email == userLogin.Email && 
            user.Password == _passwordHasher.GetHashOfAPassword(userLogin.Password));
            return foundUser;
        }
    }
}
