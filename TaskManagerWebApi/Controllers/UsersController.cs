using System.Security.Claims;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerModels;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly IAppCache _appCache;

        public UsersController(TaskManagerContext context, IAppCache appCache)
        {
            _context = context;
            _appCache = appCache;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAuthorizedUser()
        {
            return await _appCache.GetOrAddAsync<ActionResult<User>>("authorizedUser", async entry =>
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    return BadRequest();
                }
                var user = await _context.Users.FindAsync(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value));
                if (user == null)
                {
                    return BadRequest();
                }

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return Ok(user);
            });
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            return await _appCache.GetOrAddAsync<ActionResult<User>>("user", async entry =>
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return Ok(user);
            });
        }
        
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        
            return NoContent();
        }
        
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var user = (await GetAuthorizedUser()).Value;
            if (user == null)
            {
                return NotFound();
            }
        
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        
            return NoContent();
        }

        //
    }
}
