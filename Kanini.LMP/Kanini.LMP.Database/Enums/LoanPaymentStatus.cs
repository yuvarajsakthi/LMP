using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Enums
{
    public enum LoanPaymentStatus
    {
        Issued,           // Just disbursed, starting
        Current,          // On-time payments
        InGracePeriod,    // Slightly late, within grace period
        Late30Days,       // 30 days past due
        Late90Days,       // 90 days past due
        ChargedOff,       // Deemed uncollectible (Equivalent to 'Charged Off' on the chart)
        Default,          // Failed to meet terms
        FullyPaid         // Loan is closed, principal and interest paid
    }
}
