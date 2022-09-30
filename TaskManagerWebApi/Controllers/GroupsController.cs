using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerModels;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using TaskManagerWebApi.Repositories;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly AuthorizedUserRepository _authorizedUserRepository;
        private readonly TaskManagerContext _context;

        public GroupsController(TaskManagerContext context, AuthorizedUserRepository authorizedUserRepository)
        {
            _context = context;
            _authorizedUserRepository = authorizedUserRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetGroupsForRegisteredUser()
        {
            try
            {
                var user = _authorizedUserRepository.GetAuthorizedUser();

                var userGroups = await _context.GroupUsers.Include(x => x.Group)
                                .Where(x => x.User.Id == user.Id).Select(x => x.Group).ToListAsync();

                return userGroups;
            }
            catch(ArgumentException)
            {
                return NotFound("Authorized user was not found");
            }
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Group>> GetGroupInfo(Guid Id)
        {
            if(!GroupExists(Id))
                return NotFound("Group with current Id not Found");
            var entity = await _context.Groups.FindAsync(Id);
            return Ok(entity);
        }

        [Authorize(Roles = RoleType.Admin)]
        [HttpPost]
        public async Task<ActionResult<Group>> AddGroup(Group group)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (_context.Groups.Where(gr => gr.Caption == group.Caption).Count() != 0)
                {
                    return BadRequest("Group with this caption already exists");
                }
                _context.Groups.Add(group);

                GroupUser groupUser = new GroupUser()
                {
                    Id = Guid.NewGuid(),
                    User = await _context.Users.FindAsync(_authorizedUserRepository.GetAuthorizedUser().Id),
                    Group = group,
                    IsCreator = true
                };
                _context.GroupUsers.Add(groupUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetGroupInfo", new { id = group.Id }, group);
            }
            catch(ArgumentException)
            {
                return NotFound("Authorized user was not found");
            }
        }

        [Authorize(Roles = RoleType.Admin)]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(Guid Id)
        {
            var entity = await _context.Groups.FindAsync(Id);
            if (entity == null)
                return BadRequest("Group with current Id not Found");
            _context.Groups.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();

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
            _context.SaveChangesAsync();

            return group;
        }
        private bool GroupExists(Guid id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
