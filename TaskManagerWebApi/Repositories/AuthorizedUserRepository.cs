using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerModels;
using TaskManagerWebApi.DTO;

namespace TaskManagerWebApi.Repositories
{
    public class AuthorizedUserRepository
    {
        TaskManagerContext _context;
        IHttpContextAccessor _httpAccessor;
        public AuthorizedUserRepository(TaskManagerContext context, IHttpContextAccessor httpAccessor)
        {
            _context = context;
            _httpAccessor = httpAccessor;
        }
        public UserDTO GetAuthorizedUser()
        {
            var identity = _httpAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                throw new ArgumentException("Don't have authorized user");
            }
            var user = _context.Users.Find(Guid.Parse(identity.Claims.FirstOrDefault(claim => claim.Type == "Id").Value));
            if (user == null)
            {
                throw new ArgumentException("Don't have such user");
            }
            return new UserDTO { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email};
        }
    }
}
