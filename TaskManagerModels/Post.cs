using System.ComponentModel.DataAnnotations;

namespace TaskManagerModels
{
    public class Post
    {
        public Guid Id { get; set; }
        [Required]
        public string Caption { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public DateTime Deadline { get; set; }
        public ICollection<PostUser> PostUsers { get; set; }

    }
}
