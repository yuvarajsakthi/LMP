namespace Kanini.LMP.Database.Enums
{
    public enum LoanPaymentStatus
    {
        Active = 0,
        InGracePeriod = 1,
        Late30Days = 2,
        Late60Days = 3,
        Late90Days = 4,
        ChargedOff = 5,
        FullyPaid = 6,
        Defaulted = 7
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}
