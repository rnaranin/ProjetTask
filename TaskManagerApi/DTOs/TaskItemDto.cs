using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagerApi.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set;}

        public bool IsDone { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsLate {get; set; }

    }
}