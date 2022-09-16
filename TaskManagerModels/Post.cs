using System.ComponentModel.DataAnnotations;

namespace TaskManagerModels
{
    public class Post
    {
        public Guid Id { get; set; }
        [Required]
        public string Caption { get; set; }
        public string Description { get; set; }
        public PostStatus Status { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public DateTime Deadline { get; set; }
    }

    public enum PostStatus
    {
        Done,
        NotDone,
        Deleted
    }
}

