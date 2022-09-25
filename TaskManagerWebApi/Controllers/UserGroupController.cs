using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using TaskManagerModels;

namespace TaskManagerWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public UserGroupController(TaskManagerContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("{groupId}")]
        //refactor
        public async Task<ActionResult> JoinGroup(Guid groupId)
        {
            Group group = await _context.Groups.FindAsync(groupId);
            if (group == null)
                return NotFound("Don't have group with this id");

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

            return Ok();
        }
    }
}
