using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.UserDtos
{
    public class UserCreateDTO
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public UserRoles Roles { get; set; }
    }

    public class UserDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRoles Roles { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserUpdateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public UserRoles? Roles { get; set; }
        public UserStatus? Status { get; set; }
    }

    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserRoles Roles { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}