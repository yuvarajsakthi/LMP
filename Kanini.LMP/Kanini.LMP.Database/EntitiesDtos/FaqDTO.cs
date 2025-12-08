using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDto
{
    public class FaqDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CustomerId is required")]
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; } = null!;

        public string? Answer { get; set; }

        public FaqStatus Status { get; set; } = FaqStatus.Pending;
    }
}