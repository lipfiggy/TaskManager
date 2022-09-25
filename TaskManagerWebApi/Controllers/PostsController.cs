using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerModels;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly IAppCache _appCache;

        public PostsController(TaskManagerContext context, IAppCache appCache)
        {
            _context = context;
            _appCache = appCache;
        }

        [HttpGet]
        //refactor
        public async Task<ActionResult<IEnumerable<Post>>> GetUsersPosts()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User authorizedUser = await _context.Users.FindAsync(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value));

            if(authorizedUser == null)
            {
                return BadRequest();
            }
            return await _appCache.GetOrAddAsync("usersPosts",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return  _context.PostUsers.Include(userPost => userPost.Post)
                            .Where(postUser => postUser.User.Id == authorizedUser.Id).Select(postUser => postUser.Post).ToList();
                });//?
        }

        // GET: api/Posts
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsForGroup(Guid groupId)
        {
            if (_context.Groups.FindAsync(groupId) == null)
                return NotFound("Group with this id doesn't exist");

            return await _context.Posts.Where(post => post.Group.Id == groupId).ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> ChangePostStatus(Guid id,Post post)
        {
            if (id != post.Id)
                return BadRequest();

            if(_context.Posts.Where(_post => _post.Caption == post.Caption && 
                             _post.Description == post.Description && post.Deadline == _post.Deadline
                             && _post.Created == post.Created).Count() == 0)
                return BadRequest("Employees can change only status");

            if (post.Status != PostStatus.done && post.Status != PostStatus.inProcess)
                BadRequest("Status is unappropriate");

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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
        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = RoleType.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(Guid id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = RoleType.Admin)]
        [HttpPost("{groupId}")]
        public async Task<ActionResult<Post>> AddPost(Post post, Guid groupId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(_context.Posts.Where(p => p.Group == post.Group && p.Caption == post.Caption).Count() != 0)
            {
                return BadRequest("Post with this caption already exists");
            }
            Group group = await _context.Groups.FindAsync(groupId);
            if (group == null)
                return BadRequest();
            post.Group = group;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [Authorize(Roles = RoleType.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
