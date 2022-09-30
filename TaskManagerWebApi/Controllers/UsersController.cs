using System.Security.Claims;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerModels;
using TaskManagerWebApi.Repositories;
using TaskManagerWebApi.DTO;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthorizedUserRepository _authorizedUserRepository;
        private readonly TaskManagerContext _context;
        private readonly IAppCache _appCache;

        public UsersController(TaskManagerContext context, IAppCache appCache, AuthorizedUserRepository authorizedUserRepository)
        {
            _context = context;
            _appCache = appCache;
            _authorizedUserRepository = authorizedUserRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetAuthorizedUserInfo()
        {
            try
            {
                var user = _authorizedUserRepository.GetAuthorizedUser();
                return Ok(user);
            }
            catch (ArgumentException)
            {
                return NotFound("Authorized user was not found");
            }
        }

        //with caching
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserInfo(Guid id)
        {
            return await _appCache.GetOrAddAsync<ActionResult<UserDTO>>("userById", async entry =>
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound("User wasn't found");
                }

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return Ok(new UserDTO { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email });
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<UserDTO>> EditAuthorizedUserInfo(User user)
        {
            try
            {
                if (!IsRoleRight(user.Role))
                    return BadRequest("Role must be 'user' or 'admin'");
                var authorizedUser = _authorizedUserRepository.GetAuthorizedUser();
                _context.Entry(user).State = EntityState.Detached;

                if (authorizedUser.Id != user.Id)
                {
                    return BadRequest("Can not change personal data of other users");
                }

                _context.Entry(user).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return Ok(new UserDTO { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email });
            }
            catch (ArgumentException)
            {
                return NotFound("Authorized user was not found");
            }
        }

        
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteAuthorizedUser()
        {
            try
            {
                var user = _authorizedUserRepository.GetAuthorizedUser();

                _context.Users.Remove(_context.Users.Find(user.Id));
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(ArgumentException)
            {
                return NotFound("Authorized user was not found");
            }
        }

        private bool IsRoleRight(string role)
        {
            return role == "user" || role == "admin";
        }

        //
    }
}
