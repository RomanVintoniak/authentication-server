using authentication_server.DTOs;
using authentication_server.Models;
using authentication_server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authentication_server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserService UserService;

        public UsersController(UserService userService)
        {
            UserService = userService;
        }

        [HttpGet("{id:length(24)}")]
        public async Task<IActionResult> Get(string id)
        {
            User user = await UserService.GetAsync(id);

            if (user is null)
            {
                return NotFound("There is no user with this id");
            }

            UserDTO userToReturn = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsEmailConfirmed = user.IsEmailConfirmed,
                TwoFactorAuthSecret = user.TwoFactorAuthSecret,
                PasswordResetToken = user.PasswordResetToken,
                PasswordResetTokenExpires = user.PasswordResetTokenExpires,
            };

            return Ok(userToReturn);
        }
    }
}
