using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "NotificationMessage is required")]
        public string NotificationMessage { get; set; } = null!;
        public string? UserName { get; set; }
    }
}
