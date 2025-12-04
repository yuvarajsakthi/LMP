using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Authentication;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByNameAsync(string username);
        Task<UserDTO?> GetUserByIdAsync(int userId);
        Task<UserDTO?> GetUserByEmailAsync(string email);
        Task<UserDTO> RegisterCustomerAsync(CustomerRegistrationDTO registrationDto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task<UserDTO> UpdateUserAsync(UserDTO userDto);
        Task ActivateUserAsync(int userId);
        Task<bool> ResetPasswordAsync(string email, string oldPassword, string newPassword);
    }
}