using DemoCleanArchitecture.Domain.Entities;
using DemoCleanArchitecture.Domain.Interfaces;
using DemoCleanArchitecture.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace DemoCleanArchitecture.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitofwork;
        private readonly SmtpClient _smtpClient;


        public UserRepository(IConfiguration configuration, IUnitOfWork unitofwork, SmtpClient smtpClient)
        {
            _config = configuration;
            _unitofwork = unitofwork;
            _smtpClient = new SmtpClient
            {
                Host = _config["SmtpConfig:SmtpServer"],
                Port = int.Parse(_config["SmtpConfig:Port"]),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                   _config["SmtpConfig:Username"],
                   _config["SmtpConfig:Password"]
               ),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        public async Task<User?> Create(User registerViewModel)
        {
            var newUser = new User
            {
                UserName = registerViewModel.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(registerViewModel.Password),
                Email = registerViewModel.Email,
                RoleId = 2
            };

            await _unitofwork.Repository<User>().CreateAsync(newUser);
            await _unitofwork.Complete();

            //_context.Users.Add(newUser);
            //await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<User?> GetByEmail(string email)
        {
            Expression<Func<User, bool>> filter = u => u.Email == email;
            var user = await _unitofwork.Repository<User>().Get(filter);
            //from user in await _unitofwork.Repository<User>().GetAllAsync()
            //              where user.Email == email
            //              select user;
            return (User?)user;
        }

        public async Task<User?> GetById(int id)
        {
            return await _unitofwork.Repository<User>().GetByIdAsync(id);
        }

        public async Task<User?> GetByUsername(string username)
        {
            //using (var uow = _unitofwork.Begin(requiresNew: true, isTransactional: true))
            //{
            //... 
            Expression<Func<User, bool>> filter = u => u.UserName == username;
            var user = await _unitofwork.Repository<User>().Get(filter);
            return user;


            //    await uow.CompleteAsync();
            //}

            //return await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
        }

        public async Task<string> Login(User loginViewModel)
        {
            Expression<Func<User, bool>> filter = u => u.Email == loginViewModel.Email;
            var user = await _unitofwork.Repository<User>().Get(filter);
            //var roleId = await _context.Roles.Where(r => r.Id == user.RoleId).Select(r => r.Name).FirstOrDefaultAsync();
            if (user != null && BCrypt.Net.BCrypt.Verify(loginViewModel.Password, user.Password))
            {
                // Nếu xác thực thành công, tạo JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"] ?? "");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.RoleId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(30),
                    SigningCredentials = new SigningCredentials
                        (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                return jwtToken;
            }
            else
            {
                throw new ArgumentException("Wrong email or password");
            }
        }
        public async Task AddRequest(ForgotPasswordRequest request)
        {
            await _unitofwork.Repository<ForgotPasswordRequest>().CreateAsync(request);
            await _unitofwork.Complete();
        }

        public async Task DeleteExpiredRequests()
        {
            var currentTime = DateTime.UtcNow;
            var expiredRequests = await _unitofwork.Repository<ForgotPasswordRequest>().GetAll(filter: f => f.ExpirationTime < currentTime);
            if (expiredRequests.Any())
            {
                _unitofwork.Repository<ForgotPasswordRequest>().DeleteRange(expiredRequests);
                await _unitofwork.Complete();
            }
        }

        public async Task<ForgotPasswordRequest> GetByUserNameAndCode(string username, string code)
        {
            return await _unitofwork.Repository<ForgotPasswordRequest>().Get(filter: f => f.UserName == username && f.Code == code);
        }

        public async Task<bool> IsUserExists(string username)
        {
            var userExist = await _unitofwork.Repository<ForgotPasswordRequest>().Get(filter: f => f.UserName == username);
            if (userExist == null)
            {
                return false;
            }
            return true;
        }
        public void SendConfirmationEmail(string email, string confirmationCode)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["SmtpConfig:Username"]),
                Subject = "Xác nhận quên mật khẩu",
                Body = $"Mã xác nhận của bạn là: {confirmationCode}",
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            _smtpClient.Send(mailMessage);
        }

    }
}
