using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class UserRepository : ILMPRepository<User, int>
    {
        public Task<User> AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
