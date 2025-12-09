using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class UpdateStatusDTO
    {
        public ApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}
