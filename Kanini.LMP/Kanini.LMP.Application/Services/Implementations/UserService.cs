using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.UserDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private static readonly Dictionary<string, (string Token, DateTime Expiry)> _resetTokens = new();

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<UserDTO?> GetUserByNameAsync(StringDTO username) =>
            _mapper.Map<UserDTO>(await _unitOfWork.Users.GetAsync(u => u.Email == username.Value));

        public async Task<UserDTO?> GetUserByIdAsync(IdDTO userId) =>
            _mapper.Map<UserDTO>(await _unitOfWork.Users.GetByIdAsync(userId.Id));

        public async Task<UserDTO?> GetUserByEmailAsync(StringDTO email) =>
            _mapper.Map<UserDTO>(await _unitOfWork.Users.GetAsync(u => u.Email == email.Value));

        public async Task<UserDTO> RegisterCustomerAsync(CustomerRegistrationDTO registrationDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (await _unitOfWork.Users.GetAsync(u => u.Email == registrationDto.Email) != null)
                    throw new InvalidOperationException("Email already registered");

                var user = _mapper.Map<User>(registrationDto);
                user.PasswordHash = PasswordService.HashPassword(registrationDto.Password);
                user.Roles = UserRoles.Customer;
                user.Status = UserStatus.Pending;
                user.CreatedAt = DateTime.UtcNow;

                var createdUser = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var customer = _mapper.Map<Customer>(registrationDto);
                customer.UserId = createdUser.UserId;
                customer.PANNumber = registrationDto.PANNumber;
                customer.AadhaarNumber = registrationDto.AadhaarNumber;
                customer.Occupation = string.Empty;
                customer.ProfileImage = Array.Empty<byte>();
                customer.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<UserDTO>(createdUser);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<BoolDTO> ForgotPasswordAsync(StringDTO email)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == email.Value);
            if (user == null) return new BoolDTO { Value = false };

            var resetToken = Guid.NewGuid().ToString();
            _resetTokens[email.Value] = (resetToken, DateTime.UtcNow.AddHours(1));

            await _emailService.SendEmailAsync(new Database.EntitiesDto.Email.EmailDto
            {
                To = email.Value,
                Subject = "Password Reset Request",
                Body = $"Dear {user.FullName},\n\nYour password reset token is: {resetToken}\n\nThis token will expire in 1 hour."
            });
            return new BoolDTO { Value = true };
        }

        public async Task<UserDTO> CreateUserAsync(UserCreateDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = PasswordService.HashPassword(userDto.Password);
            user.CreatedAt = DateTime.UtcNow;

            var createdUser = await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDTO>(createdUser);
        }

        public async Task<UserDTO> UpdateUserAsync(UserUpdateDTO userDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userDto.UserId);
            if (user == null) throw new InvalidOperationException("User not found");

            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            if (userDto.Roles.HasValue) user.Roles = userDto.Roles.Value;
            if (userDto.Status.HasValue) user.Status = userDto.Status.Value;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDTO>(updatedUser);
        }

        public async Task ActivateUserAsync(IdDTO userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId.Id);
            if (user == null) throw new InvalidOperationException("User not found");

            user.Status = UserStatus.Active;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BoolDTO> ResetPasswordAsync(StringDTO email, StringDTO oldPassword, StringDTO newPassword)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == email.Value);
            if (user == null) return new BoolDTO { Value = false };

            user.PasswordHash = PasswordService.HashPassword(newPassword.Value);
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return new BoolDTO { Value = true };
        }
    }
}