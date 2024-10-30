using authentication_server.Configurations;
using authentication_server.DTOs;
using authentication_server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace authentication_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IMongoCollection<User> usersCollection;

        public AuthController(IOptions<DatabaseSettings> databaseSettings)
        {
            MongoClient mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            IMongoDatabase mongoDB = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            usersCollection = mongoDB.GetCollection<User>(databaseSettings.Value.CollectionName);
        }


        [HttpPost("Registration")]
        public IActionResult Registration(UserRegistrationDTO user)
        {
            if (user.Password == user.PasswordConfirm)
            {
                User userFromDB = usersCollection.Find(u => u.Email == user.Email).FirstOrDefault();

                if (userFromDB is null)
                {
                    User newUser = new()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password)),
                    };

                    usersCollection.InsertOne(newUser);

                    return Created();
                }

                return BadRequest($"User with this email already exists");
            }

            return BadRequest("Passwords don't match");
        }


        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO user)
        {
            User userFromDB = usersCollection.Find(u => u.Email == user.Email).FirstOrDefault();

            if (userFromDB is not null)
            {
                byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password));

                if (AreHashesEqual(userFromDB.PasswordHash, passwordHash))
                {
                    return Ok( new { id = userFromDB.Id } );
                }
            }

            return BadRequest("Email or password is incorrect");
        }


        [HttpPost("DefaultUser")]
        public IActionResult PostDefaultUser()
        {
            string password = "123789";
            byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            User user = new()
            {
                FirstName = "John",
                LastName = "Smit",
                Email = "johnsmit@gmail.com",
                PasswordHash = passwordHash,
                IsEmailConfirmed = false,
                PasswordResetTokenExpires = DateTime.UtcNow.AddMinutes(30)
            };

            usersCollection.InsertOne(user);

            return Ok(user);
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
