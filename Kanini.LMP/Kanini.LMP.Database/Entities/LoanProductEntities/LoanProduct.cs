using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.Entities.LoanProductEntities
{
    [Index(nameof(LoanType), IsUnique = true)]
    public class LoanProduct
    {
        [Key]
        public int LoanProductId { get; set; }
        [Required]
        public LoanType LoanType { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
