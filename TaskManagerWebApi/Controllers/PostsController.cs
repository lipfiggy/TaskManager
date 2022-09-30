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
using Microsoft.Extensions.Hosting;
using TaskManagerModels;
using TaskManagerWebApi.Repositories;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AuthorizedUserRepository _authorizedUserRepository;
        private readonly TaskManagerContext _context;
        private readonly IAppCache _appCache;

        public PostsController(TaskManagerContext context, IAppCache appCache, AuthorizedUserRepository authorizedUserRepository)
        {
            _context = context;
            _appCache = appCache;
            _authorizedUserRepository = authorizedUserRepository;
        }

        [HttpGet]
        //with caching
        public async Task<ActionResult<IEnumerable<Post>>> GetUsersPosts()
        {
            return await _appCache.GetOrAddAsync<ActionResult<IEnumerable<Post>>>("usersPosts",
                async entry =>
                {
                    try
                    {
                        var authorizedUser = _authorizedUserRepository.GetAuthorizedUser();
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                        return _context.PostUsers.Include(userPost => userPost.Post)
                                .Where(postUser => postUser.User.Id == authorizedUser.Id).Select(postUser => postUser.Post).ToList();
                    }
                    catch(ArgumentException)
                    {
                        return NotFound("Authorized user was not found");
                    }
                });
        }

        // GET: api/Posts
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsForGroup(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
                return NotFound("Group with this id doesn't exist");

            return await _context.Posts.Where(post => post.Group.Id == groupId).ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPostInfo(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPut("status/{id}")]
        public async Task<ActionResult<Post>> ChangePostStatus(Guid id,Post post)
        {
            if (id != post.Id)
                return BadRequest();

            if (!IsStatusRight(post.Status))
            {
                return BadRequest("Status can be only in process or done");
            }

            if (_context.Posts.Where(_post => _post.Caption == post.Caption && 
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
            return post;
        }
        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = RoleType.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Post>> PutPost(Guid id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!IsStatusRight(post.Status))
            {
                return BadRequest("Status can be only in process or done");
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

            return post;
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

            if (!IsStatusRight(post.Status))
            {
                return BadRequest("Status can be only in process or done");
            }

            Group group = await _context.Groups.FindAsync(groupId);
            if (group == null)
                return BadRequest();
            post.Group = group;

            if (_context.Posts.Any(p => p.Group == post.Group && p.Caption == post.Caption))
            {
                return BadRequest("Post with this caption already exists");
            }
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostInfo", new { id = post.Id }, post);
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

        private bool IsStatusRight(string status)
        {
            return status == "in process" || status == "done";
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
