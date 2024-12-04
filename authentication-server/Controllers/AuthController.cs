using authentication_server.Configurations;
using authentication_server.DTOs;
using authentication_server.Interfaces;
using authentication_server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace authentication_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public AuthController(
            IJwtService jwtService,
            IEmailService emailService,
            IUserRepository userRepository
        )
        {
            _jwtService = jwtService;
            _emailService = emailService;
            _userRepository = userRepository;
        }


        [HttpPost("Registration")]
        public async Task<ActionResult> Registration(UserRegistrationDTO user)
        {
            if (user.Password == user.PasswordConfirm)
            {
                User userFromDB = await _userRepository.GetByEmailAsync(user.Email);

                if (userFromDB is null)
                {
                    User newUser = new()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password)),
                    };

                    await _userRepository.CreateAsync(newUser);

                    return Created();
                }

                return BadRequest($"User with this email already exists");
            }

            return BadRequest("Passwords don't match");
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserLoginDTO user)
        {
            User userFromDB = await _userRepository.GetByEmailAsync(user.Email);

            if (userFromDB is not null)
            {
                byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password));

                if (AreHashesEqual(userFromDB.PasswordHash, passwordHash))
                {
                    Claim[] claims = [
                        new Claim("userId", userFromDB.Id),
                        new Claim("email", userFromDB.Email)
                    ];

                    string jwtToken = _jwtService.GetToken(claims, DateTime.Now.AddHours(1));

                    await _emailService.SendEmailVerificationAsync(userFromDB.Email);

                    return Ok(new { token = jwtToken, userId = userFromDB.Id} );
                }
            }

            return BadRequest("Email or password is incorrect");
        }


        [HttpGet("Email-verification/{token}")]
        public async Task<ActionResult> VerifyEmail(string token)
        {
            if(!_jwtService.IsTokenValid(token))
            {
                return BadRequest();
            }

            string? userEmail = _jwtService.GetUserEmailFromToken(token);

            if (userEmail is null)
            {
                return BadRequest();
            }

            User user = await _userRepository.GetByEmailAsync(userEmail);

            if (user is null)
            {
                return NotFound();
            }

            user.IsEmailConfirmed = true;
            await _userRepository.UpdateAsync(user);

            return Ok();
        }
        

        private static bool AreHashesEqual(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
            {
                return false;
            }

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
