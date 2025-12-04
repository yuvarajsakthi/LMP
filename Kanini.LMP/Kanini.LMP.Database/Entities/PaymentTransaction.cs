using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kanini.LMP.Database.Entities
{
    public class PaymentTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        [ForeignKey(nameof(EMIPlan))]
        public int EMIId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.UPI;
        
        [MaxLength(100)]
        public string? TransactionReference { get; set; }
       
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        [ForeignKey(nameof(LoanAccount))]
        public int LoanAccountId { get; set; }
    }
}
