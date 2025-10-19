namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto
{
    internal class ApplicationTrendDto
    {
        public string PeriodLabel { get; set; } //  "Oct 24 - Oct 30"
        public DateTime PeriodStartDate { get; set; }
        
        public decimal MetricValue { get; set; }//  the total count 
    }
}
