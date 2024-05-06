using DemoCleanArchitecture.Domain.Entities;
using DemoCleanArchitecture.Domain.Interfaces;
using DemoCleanArchitecture.Domain.Interfaces.IServices;
using DemoCleanArchitecture.Infrastructure.MessageQueue;
using DemoCleanArchitecture.WebApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DemoCleanArchitecture.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private IUserService _userService;


        public UserController(IUserService userService, IMessageQueue messageQueue, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _userService = userService;
            _messageQueue = messageQueue;
            _backgroundTaskQueue = backgroundTaskQueue;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            User newUser = new()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Username,
                Password = registerViewModel.Password,

            };
            try
            {
                User? user = await _userService.Register(newUser);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            User user = new User
            {
                Email = loginViewModel.Email,
                Password = loginViewModel.Password
            };
            try
            {
                string jwtToken = await _userService.Login(user);
                return Ok(new { jwtToken });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpPost("SendForgotPasswordEmail")]
        public async Task<IActionResult> SendForgotPasswordEmail([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (serviceScopeFactory, cancellationToken) =>
                {
                    // Get services
                    using var scope = serviceScopeFactory.CreateScope();
                    var myService = scope.ServiceProvider.GetRequiredService<IUserService>();
                    await myService.SendForgotPasswordEmail(request);
                    
                });

                // Trả về thành công
                return Ok(new { message = "A confirmation email has been sent to your email address." });
            }
            catch (Exception)
            {
                // Xử lý nếu có lỗi xảy ra
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        [AllowAnonymous]
        [HttpPost("VerifyCodeResetPassword")]
        public async Task<ActionResult> VerifyCodeResetPassword([FromBody] ForgotPasswordRequest request)
        {
            await _userService.VerifyCodeResetPassword(request);
            return Ok(new { message = "Comfirm successfully" });
        }

        [AllowAnonymous]
        [HttpDelete("DeleteConfirmCodeExpired")]
        public async Task<ActionResult> DeleteConfirmCodeExpired()
        {
            await _userService.DeleteExpiredRequests();
            return Ok(new { message = "Delete successfully" });
        }
    }
}
