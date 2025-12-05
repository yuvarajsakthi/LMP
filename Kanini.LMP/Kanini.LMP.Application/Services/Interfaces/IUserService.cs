using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.UserDtos;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByNameAsync(StringDTO username);
        Task<UserDTO?> GetUserByIdAsync(IdDTO userId);
        Task<UserDTO?> GetUserByEmailAsync(StringDTO email);
        Task<UserDTO> RegisterCustomerAsync(CustomerRegistrationDTO registrationDto);
        Task<BoolDTO> ForgotPasswordAsync(StringDTO email);
        Task<UserDTO> CreateUserAsync(UserCreateDTO userDto);
        Task<UserDTO> UpdateUserAsync(UserUpdateDTO userDto);
        Task ActivateUserAsync(IdDTO userId);
        Task<BoolDTO> ResetPasswordAsync(StringDTO email, StringDTO oldPassword, StringDTO newPassword);
    }
}