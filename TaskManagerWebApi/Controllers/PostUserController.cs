using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerModels;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostUserController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public PostUserController(TaskManagerContext context)
        {
            _context = context;
        }

        [HttpPost("{postId}")]
        //refactor
        public async Task<ActionResult> JoinPost(Guid postId)
        {
            Post post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return NotFound("Don't have group with this id");

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            PostUser postUser = new PostUser()
            {
                Id = Guid.NewGuid(),
                Post = post,
                User = await _context.Users.FindAsync(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value))
            };
            _context.PostUsers.Add(postUser);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
