using System.ComponentModel.DataAnnotations;

namespace TaskManagerModels
{
    public class Post
    {
        public Guid Id { get; set; }
        [Required]
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public DateTime Deadline { get; set; }

        public Group Group { get; set; }
    }

    public class PostStatus
    {
        public const string done = "done";
        public const string inProcess = "in processs";
    }
}

