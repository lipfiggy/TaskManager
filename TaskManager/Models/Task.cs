using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class Task
    {
        public Guid id { get; set; }
        [Required]
        public string Caption { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public DateTime Deadline { get; set; }

    }
}
