using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Authentication
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

    }
}
