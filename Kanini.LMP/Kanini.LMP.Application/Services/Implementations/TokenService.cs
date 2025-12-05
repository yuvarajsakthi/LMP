using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.UserDtos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration config, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _config = config;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public StringDTO GenerateToken(UserDTO userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.UserId.ToString()),
                new Claim(ClaimTypes.Name, userDto.FullName),
                new Claim(ClaimTypes.Role, userDto.Roles.ToString())
            };

            if (userDto.Roles == Database.Enums.UserRoles.Customer)
            {
                var customer = _unitOfWork.Customers.GetAsync(c => c.UserId == userDto.UserId).Result;
                if (customer != null)
                {
                    claims.Add(new Claim(Application.Constants.ApplicationConstants.Claims.CustomerId, customer.CustomerId.ToString()));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new StringDTO { Value = new JwtSecurityTokenHandler().WriteToken(token) };
        }

        public async Task<StringDTO?> AuthenticateAsync(StringDTO email, StringDTO password)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == email.Value);
            if (user == null)
                return null;

            if (user.Status != Database.Enums.UserStatus.Active)
                return null;

            if (!string.IsNullOrEmpty(password.Value) && !PasswordService.VerifyPassword(password.Value, user.PasswordHash))
                return null;

            return GenerateToken(_mapper.Map<UserDTO>(user));
        }
    }
}
