using DemoCleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCleanArchitecture.Domain.Interfaces.IServices
{
    public interface IUserService
    {
        Task<User?> GetUserById(int userId);
        Task<User?> Register(User registerViewModel);
        //jwt string
        Task<string> Login(User loginViewModel);
        Task SendForgotPasswordEmail(ForgotPasswordRequest request);
        Task VerifyCodeResetPassword(ForgotPasswordRequest request);
        Task DeleteExpiredRequests();
    }
}
