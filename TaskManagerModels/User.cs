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
        public string Role { get; set; }
        public ICollection<GroupUser> GroupUsers { get; set; } = null!;

    }
}

public static class RoleType
{
    public const string Admin = "admin";
    public const string User = "user";
}