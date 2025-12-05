using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
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

        public async Task<UserDTO?> GetUserByNameAsync(string username) =>
            _mapper.Map<UserDTO>(await _unitOfWork.Users.GetAsync(u => u.Email == username));

        public async Task<UserDTO?> GetUserByIdAsync(int userId) =>
            _mapper.Map<UserDTO>(await _unitOfWork.Users.GetByIdAsync(userId));

        public async Task<UserDTO?> GetUserByEmailAsync(string email) =>
            _mapper.Map<UserDTO>(await _unitOfWork.Users.GetAsync(u => u.Email == email));

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
                customer.AadhaarNumber = string.Empty;
                customer.Occupation = string.Empty;
                customer.PANNumber = string.Empty;
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

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == email);
            if (user == null) return false;

            var resetToken = Guid.NewGuid().ToString();
            _resetTokens[email] = (resetToken, DateTime.UtcNow.AddHours(1));

            await _emailService.SendEmailAsync(new Database.EntitiesDto.Email.EmailDto
            {
                ToEmail = email,
                Subject = "Password Reset Request",
                Body = $"Dear {user.FullName},\n\nYour password reset token is: {resetToken}\n\nThis token will expire in 1 hour."
            });
            return true;
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = PasswordService.HashPassword(userDto.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            var createdUser = await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDTO>(createdUser);
        }

        public async Task<UserDTO> UpdateUserAsync(UserDTO userDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userDto.UserId);
            if (user == null) throw new InvalidOperationException("User not found");

            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDTO>(updatedUser);
        }

        public async Task ActivateUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new InvalidOperationException("User not found");

            user.Status = UserStatus.Active;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> ResetPasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == email);
            if (user == null) return false;

            user.PasswordHash = PasswordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}