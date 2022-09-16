﻿using System.ComponentModel.DataAnnotations;

namespace TaskManagerModels
{
    public class Group
    {
        public Guid Id { get; set; }
        [Required]
        public string Caption { get; set; }
        public string Description { get; set; }
    }
}
