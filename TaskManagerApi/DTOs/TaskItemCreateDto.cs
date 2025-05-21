using System.ComponentModel.DataAnnotations;
using TaskManagerApi.Attributes;

namespace TaskManagerApi.DTOs
{
    public class TaskItemCreateDto
    {
        [Required(ErrorMessage ="Le titre est obligatoire")]
        [StringLength(100, MinimumLength =3, ErrorMessage = "Le titre doit contenir entre 3 et 100 caractères.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }
        
        [FutureDate(ErrorMessage = "La date d'échéance ne peut pas être dans le passé.")]
        public DateTime? DueDate { get; set; }
    }
}