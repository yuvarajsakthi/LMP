using Kanini.LMP.Database.Enums;
using System;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class LoanApplicationBaseDto
    {
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public LoanType LoanProductType { get; set; }
        public decimal RequestedAmount { get; set; }
        public int TenureMonths { get; set; }
        public DateTime AppliedDate { get; set; }
        public decimal? MonthlyInstallment { get; set; }
        public string? DocumentName { get; set; }
        public byte[]? DocumentData { get; set; }
        public byte[]? SignatureImage { get; set; }
        public string RelationFullName { get; set; } = null!;
        public string RelationshipWithApplicant { get; set; } = null!;
        public int MobileNumber { get; set; }
        public string RelationAddress { get; set; } = null!;
        public string PresentAddress { get; set; } = null!;
        public string PermanentAddress { get; set; } = null!;
        public string District { get; set; } = null!;
        public IndianStates State { get; set; }
        public int ZipCode { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsActive { get; set; }
    }
}
