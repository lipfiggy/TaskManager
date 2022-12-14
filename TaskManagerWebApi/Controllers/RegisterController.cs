using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Text;
using TaskManagerModels;
using TaskManagerWebApi.DTO;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterController(TaskManagerContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel userRegister)
        {
            if (userRegister == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (!userRegister.Role.Equals(RoleType.Admin) && !userRegister.Role.Equals(RoleType.User))
                return BadRequest("Role must be 'user' or 'admin'");

            if (_context.Users.Where(user => user.Email == userRegister.Email).Any())
                return BadRequest("User with this email already exists");

            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = userRegister.Email,
                Password = _passwordHasher.GetHashOfAPassword(userRegister.Password),
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                Role = userRegister.Role
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            TrySendConfirmationEmail(userRegister.Email,userRegister.FirstName);
            return Ok(new UserDTO { Id = newUser.Id, FirstName = newUser.FirstName, 
                      LastName = newUser.LastName, Email = newUser.Email});
        }


        private void TrySendConfirmationEmail(string userEmail, string userName)
        {
            string senderEmail = "MyTaskManager@gmail.com";
            string subject = "Registration confirmation";
            string body = $"{userName}, you have registered successfully!";
            MailMessage message = new MailMessage(senderEmail, userEmail, subject, body);
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            NetworkCredential myCredentials = new NetworkCredential("nedavnaia@gmail.com", "hbbzxzzpnrhxkzms");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = myCredentials;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
