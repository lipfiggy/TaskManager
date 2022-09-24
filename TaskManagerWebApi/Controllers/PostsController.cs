using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public async Task<ActionResult<IEnumerable<Post>>> GetUsersPosts()
        {
            UsersController controller = new UsersController(_context, _appCache);
            User authorizedUser = (await controller.GetAuthorizedUser()).Value;
            if(authorizedUser == null)
            {
                return BadRequest();
            }
            return await _appCache.GetOrAddAsync(null,
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return await _context.PostUsers.Where(postUser => postUser.User.Id == authorizedUser.Id).Select(postUser => postUser.Post).ToListAsync();
                });//?
        }

        // GET: api/Posts
        [HttpGet("groupId")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsForGroup(Guid groupId)
        {
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

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "manager")]
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
        [Authorize(Roles = "manager")]
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [Authorize(Roles = "manager")]
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
