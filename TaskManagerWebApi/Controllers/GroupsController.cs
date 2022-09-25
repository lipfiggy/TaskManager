using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerModels;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public GroupsController(TaskManagerContext context) =>
            _context = context;

        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetGroupsForRegisteredUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = await _context.Users.FindAsync(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value));


            if (user == null)
                return NotFound();

            var userGroups = await _context.GroupUsers.Include(x => x.Group)
                            .Where(x=> x.User.Id == user.Id).Select(x => x.Group).ToListAsync();

            return userGroups;
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Group>> GetGroupInfo(Guid Id)
        {
            var entity = await _context.Groups.FindAsync(Id);
            if (entity == null)
                return BadRequest("Group with current Id not Found");
            return Ok(entity);
        }

        [Authorize(Roles = RoleType.Admin)]
        [HttpPost]
        public async Task<ActionResult<Group>> AddGroup(Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(_context.Groups.Where(gr => gr.Caption == group.Caption).Count() != 0)
            {
                return BadRequest("Group with this caption already exists");
            }
            _context.Groups.Add(group);
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            GroupUser groupUser = new GroupUser()
            {
                Id = Guid.NewGuid(),
                User = await _context.Users.FindAsync(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value)),
                Group = group,
                IsCreator = true
            };
            _context.GroupUsers.Add(groupUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroupInfo", new { id = group.Id }, group);
        }

        [Authorize(Roles = RoleType.Admin)]
        [HttpDelete("{Id}")]
        public async Task<ActionResult<Group>> Delete(Guid Id)
        {
            var entity = await _context.Groups.FindAsync(Id);
            if (entity == null)
                return BadRequest("Group with current Id not Found");
            _context.Groups.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok(entity);

        }

        [Authorize(Roles = RoleType.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Group>> EditGroupInfo(Guid id, Group group)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (id != group.Id)
            {
                return BadRequest();
            }

            _context.Entry(group).State = EntityState.Modified;

            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                GroupUser groupUser = new GroupUser()
                {
                    Id = Guid.NewGuid(),
                    User = await _context.Users.FindAsync(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value)),
                    Group = group,
                    IsCreator = false
                };
                _context.GroupUsers.Add(groupUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool GroupExists(Guid id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
