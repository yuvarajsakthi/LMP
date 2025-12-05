using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.Entities
{
    public class Faq
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string Question { get; set; } = null!;

        public string? Answer { get; set; }

        public FaqStatus Status { get; set; } = FaqStatus.Pending;
    }
}