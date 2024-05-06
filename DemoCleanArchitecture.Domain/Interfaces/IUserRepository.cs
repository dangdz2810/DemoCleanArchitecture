using DemoCleanArchitecture.Domain.Entities;

namespace DemoCleanArchitecture.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetById(int id);
        Task<User?> GetByUsername(string username);
        Task<User?> GetByEmail(string email);

        Task<User?> Create(User registerViewModel);
        Task<string> Login(User loginViewModel);
        Task<bool> IsUserExists(string username);
        Task AddRequest(ForgotPasswordRequest request);
        Task<ForgotPasswordRequest> GetByUserNameAndCode(string username, string code);
        Task DeleteExpiredRequests();
        void SendConfirmationEmail(string email, string confirmationCode);
    }
}
