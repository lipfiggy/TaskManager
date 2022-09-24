using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


using System.Text;
using TaskManagerModels;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public RegisterController(TaskManagerContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel userRegister)
        {
            if (userRegister == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (!userRegister.Role.Equals(RoleType.Admin) && !userRegister.Role.Equals(RoleType.User))
                return BadRequest("Role must be 'user' or 'admin'");

            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = userRegister.Email,
                Password = userRegister.Password,
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                Role = userRegister.Role
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();


            return Ok(newUser);
        }

    }
}
