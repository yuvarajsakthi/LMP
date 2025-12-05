using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.UserDtos;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ITokenService
    {
        StringDTO GenerateToken(UserDTO userDto);
        Task<StringDTO?> AuthenticateAsync(StringDTO email, StringDTO password);
    }
}
