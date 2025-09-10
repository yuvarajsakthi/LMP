using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        [Required, MaxLength(100)]
        [DisplayName("First Name")]
        public string FirstName { get; set; } = null!;
        [Required, MaxLength(100)]
        [DisplayName("Last Name")]
        public string LastName { get; set; } = null!;
        [DisplayName("Email")]
        [Required(ErrorMessage = "The Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;
        [Required]
        [DisplayName("Password")]
        public string PasswordHash { get; set; } = null!;
        [DefaultValue(false)]
        public bool IsAdmin { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get;set; }
    }
}
