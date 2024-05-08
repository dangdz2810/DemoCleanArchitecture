using DemoCleanArchitecture.Domain.Entities;
using DemoCleanArchitecture.Domain.Interfaces.IServices;
using DemoCleanArchitecture.Domain.Interfaces;
using DemoCleanArchitecture.Application.Exceptions;

namespace DemoCleanArchitecture.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _userRepository.GetById(userId);
        }

        public async Task<string> Login(User loginViewModel)
        {
            return await _userRepository.Login(loginViewModel);
        }

        public async Task<User?> Register(User registerViewModel)
        {
            // Kiểm tra xem username hoặc email đã tồn tại trong database chưa
            //Tạo ra đối tượng User từ RegisterViewModel            
            var existingUserByUsername = await _userRepository
                        .GetByUsername(registerViewModel.UserName ?? "");
            if (existingUserByUsername != null)
            {
                throw new ArgumentException("Username already exists");
            }

            var existingUserByEmail = await _userRepository
                .GetByEmail(registerViewModel.Email);
            if (existingUserByEmail != null)
            {
                throw new ArgumentException("Email already exists");
            }

            // Thực hiện thêm mới user
            return await _userRepository.Create(registerViewModel);
        }
        public async Task DeleteExpiredRequests()
        {
            await _userRepository.DeleteExpiredRequests();
        }

        public async Task SendForgotPasswordEmail(ForgotPasswordRequest request)
        {
            var isUserExists = await _userRepository.GetByUsername(request.UserName);


            if (isUserExists is null)
            {
                throw new DataNotFoundException("User is not existed");
            }

            var confirmationCode = GenerateRandomCode();

            request.ExpirationTime = DateTime.UtcNow.AddMinutes(2);
            request.Code = confirmationCode;

            await _userRepository.AddRequest(request);
            _userRepository.SendConfirmationEmail(isUserExists.Email, confirmationCode);
        }

        public async Task VerifyCodeResetPassword(ForgotPasswordRequest request)
        {
            var confirmCode = await _userRepository.GetByUserNameAndCode(request.UserName, request.Code);

            if (confirmCode is null)
            {
                throw new DataNotFoundException("Confirm code is invalid");
            }

            if (confirmCode.ExpirationTime < DateTime.UtcNow)
            {
                await _userRepository.DeleteExpiredRequests();
                throw new DataNotFoundException("The code has expired");
            }

        }
        private string GenerateRandomCode()
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
