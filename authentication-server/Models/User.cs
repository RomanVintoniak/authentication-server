using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace authentication_server.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] Password { get; set; } = [];
        public byte[] PasswordSalt { get; set; } = [];
        public bool IsEmailConfirmed { get; set; } = false;
        public string? TwoFactorAuthSecret { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }
    }
}
