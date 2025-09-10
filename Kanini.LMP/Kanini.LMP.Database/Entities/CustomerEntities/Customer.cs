using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required, MaxLength(10)]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [MaxLength(250)]
        [DisplayName("Address")]
        public string? Address { get; set; }

        [MaxLength(100)]
        [DisplayName("City")]
        public string? City { get; set; }

        [MaxLength(100)]
        [DisplayName("State")]
        public string? State { get; set; }

        [MaxLength(10)]
        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
