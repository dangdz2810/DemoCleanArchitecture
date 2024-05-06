using DemoCleanArchitecture.Domain.Entities;
using DemoCleanArchitecture.Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DemoCleanArchitecture.Infrastructure.Filters
{
    public class JwtAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _config;
        private readonly string[] _roles;
        private readonly DataContext _context;
        public JwtAuthorizeFilter(IConfiguration config
            , string[] roles, DataContext context
            )
        {
            _config = config;
            _roles = roles;
            _context = context;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var token = context.HttpContext.Request
                .Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("Jwt")["SecretKey"] ?? "");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //Nếu token hết hạn,
                    //thì khi gọi phương thức ValidateToken,
                    //mã lỗi SecurityTokenExpiredException sẽ được throw ra
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    // Token đã hết hạn
                    // Xử lý lỗi hoặc đăng nhập lại để tạo mới token
                    context.Result = new UnauthorizedResult();
                    return;
                }
                var userId = int.Parse(jwtToken.Claims.First().Value);
                context.HttpContext.Items["UserId"] = userId;

                // Kiểm tra vai trò của người dùng
                List<Role> roles = _context.Roles.ToList();

                var userRoles = GetUserRoles(jwtToken);

                bool hasRequiredRole = userRoles.Intersect(_roles).Any();
                if (!hasRequiredRole)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
        private IEnumerable<string?> GetUserRoles(JwtSecurityToken jwtToken)
        {
            List<Role> roles = _context.Roles.ToList();
            return jwtToken.Claims
                .Where(c => c.Type == "role")
                .Select(c =>
                {
                    int roleId = int.Parse(c.Value);
                    var role = roles.FirstOrDefault(r => r.Id == roleId);
                    return role != null ? role.Name : null;
                })
                .Where(roleName => roleName != null);
        }
    }
}
