using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [Required]
        public string NotificationMessage { get; set; } = null!;
        public User? User { get; set; }
    }
}
