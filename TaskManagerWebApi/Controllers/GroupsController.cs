using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerModels;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public GroupsController(TaskManagerContext context) =>
            _context = context;
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetGroups()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var user = await _context.Users.FindAsync(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value));


            if (user == null)
                return NotFound();

            var userGroups = await _context.Groups.Include(x => x.GroupUsers).ThenInclude(x => x.User).Where(x=>x.Id == user.Id).ToListAsync();

            return userGroups;
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Group>> GetInfo(Guid Id)
        {
            var entity = await _context.Groups.FindAsync(Id);
            if (entity == null)
                return BadRequest("Group with current Id not Found");
            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult<Group>> AddGroup(Group group)
        {
            if(group == null)
                return Problem("Cannot create group. Invalid data");
            group.Id = Guid.NewGuid();

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            string createdPath = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path
                                                            + "/" + group.Id;
            return Created(createdPath,group);
        }

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

        [HttpPut("{Id}")]
        public async Task<ActionResult<Group>> EditDesc(Guid Id, string description)
        {
            var entity = await _context.Groups.FindAsync(Id);
            if (entity == null)
                return BadRequest("Group Not Found");
            if (description == null)
                return BadRequest("Description cannot be null");
            entity.Description = description;
            _context.Groups.Update(entity);
            await _context.SaveChangesAsync();
            return Ok(entity);
        }
    }
}
