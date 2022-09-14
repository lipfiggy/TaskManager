using System.ComponentModel.DataAnnotations;
using System.Data;

namespace TaskManagerModels
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }
        public ICollection<Group> GroupsCreated { get; set; }
        public ICollection<GroupUser> GroupUsers { get; set; }
        public ICollection<PostUser> PostUsers { get; set; }
    }
}
