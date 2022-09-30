using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerModels;
using TaskManagerWebApi.Repositories;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostUserController : ControllerBase
    {
        private readonly AuthorizedUserRepository _authorizedUserRepository;
        private readonly TaskManagerContext _context;

        public PostUserController(TaskManagerContext context, AuthorizedUserRepository authorizedUserRepository)
        {
            _context = context;
            _authorizedUserRepository = authorizedUserRepository;
        }

        [HttpPost("{postId}")]
        public async Task<ActionResult> JoinPost(Guid postId)
        {
            try
            {
                Post post = await _context.Posts.FindAsync(postId);
                if (post == null)
                    return NotFound("Don't have group with this id");

                PostUser postUser = new PostUser()
                {
                    Id = Guid.NewGuid(),
                    Post = post,
                    User = await _context.Users.FindAsync(_authorizedUserRepository.GetAuthorizedUser().Id)
                };
                _context.PostUsers.Add(postUser);
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
