namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard
{
    internal class ApplicationStatusSummaryDto
    {
        public string StatusName { get; set; } //  "Closed", "Rejected"
        public int ApplicationCount { get; set; }
        public decimal Percentage { get; set; }
       
    }
}
