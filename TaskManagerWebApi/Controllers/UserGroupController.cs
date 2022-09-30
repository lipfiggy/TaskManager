using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using TaskManagerModels;
using TaskManagerWebApi.Repositories;

namespace TaskManagerWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly AuthorizedUserRepository _authorizedUserRepository;
        private readonly TaskManagerContext _context;

        public UserGroupController(TaskManagerContext context, AuthorizedUserRepository authorizedUserRepository)
        {
            _context = context;
            _authorizedUserRepository = authorizedUserRepository;
        }

        [Authorize]
        [HttpPost("{groupId}")]
        public async Task<ActionResult> JoinGroup(Guid groupId)
        {
            try
            {
                Group group = await _context.Groups.FindAsync(groupId);
                if (group == null)
                    return NotFound("Don't have group with this id");

                GroupUser groupUser = new GroupUser()
                {
                    Id = Guid.NewGuid(),
                    User = await _context.Users.FindAsync(_authorizedUserRepository.GetAuthorizedUser().Id),
                    Group = group,
                    IsCreator = false
                };
                _context.GroupUsers.Add(groupUser);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch(ArgumentException)
            {
                return NotFound("Authorized user was not found");
            }
        }
    }
}
