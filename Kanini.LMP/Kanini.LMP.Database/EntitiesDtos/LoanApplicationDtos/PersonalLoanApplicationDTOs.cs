using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class PersonalLoanApplicationDTO : LoanApplicationBaseDto
    {
        [Required]
        public EmploymentType EmploymentType { get; set; }
        [Required]
        public decimal MonthlyIncome { get; set; }
        [Required]
        public int WorkExperienceYears { get; set; }
        [Required]
        public LoanPurposePersonal LoanPurpose { get; set; }
        public IFormFile? DocumentUpload { get; set; }
    }

    public class UpdatePersonalLoanApplicationDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        public ApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}