using Kanini.LMP.Database.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        [DisplayName("First Name")]
        public string FullName { get; set; } = null!;

        [DisplayName("Email")]
        [Required(ErrorMessage = "The Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required]
        [DisplayName("Password")]
        public string PasswordHash { get; set; } = null!;
        
        [Required]
        public UserEnums Roles { get; set; }
        
        public UserStatus Status { get; set; }
        
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get;set; }
    }
}
