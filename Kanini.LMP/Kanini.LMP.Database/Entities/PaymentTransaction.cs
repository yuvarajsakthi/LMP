using Kanini.LMP.Database.Entities.CustomerEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities
{
    public class PaymentTransaction
    {
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(EMIPlan))]
        public Guid EMIId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = "UPI"; // UPI, NetBanking, Card, Cash
        public string? TransactionReference { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public enum PaymentStatus
    {
        Pending, Success, Failed, Reversed
    }
}
