using Kanini.LMP.Data.Repositories.Interfaces.CustomerInterfaces;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;

namespace Kanini.LMP.Data.Repositories.Interfaces.LoanApplicationInterfaces
{
    public interface IPersonalLoanApplicationRepository : ILMPRepository<PersonalLoanApplication, Guid>
    {

    }
}
