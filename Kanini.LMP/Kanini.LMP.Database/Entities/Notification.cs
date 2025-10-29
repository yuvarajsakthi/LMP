using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; } = Guid.NewGuid();

        // --- Recipient and Linkage ---
        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; } // FK to the recipient (User)

        // --- Content ---
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = null!;

        // --- Status and Timestamps ---
        [Required]
        public bool IsRead { get; set; } = false; // Whether the user has viewed it

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public virtual User User { get; set; } = null!;

    }
}
