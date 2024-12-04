using authentication_server.Configurations;
using authentication_server.DTOs;
using authentication_server.Interfaces;
using authentication_server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace authentication_server.Services
{
    public class UserService : IUserRepository
    {
        private readonly IMongoCollection<User> usersCollection;

        public UserService(IOptions<DatabaseSettings> databaseSettings)
        {
            MongoClient mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            IMongoDatabase mongoDB = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            usersCollection = mongoDB.GetCollection<User>(databaseSettings.Value.CollectionName);
        }
        public async Task<User> GetByIdAsync(string id) => 
            await usersCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
        public async Task<User> GetByEmailAsync(string email) =>
            await usersCollection.Find(user => user.Email == email).FirstOrDefaultAsync();
        public async Task CreateAsync(User user) => 
            await usersCollection.InsertOneAsync(user);
        public async Task UpdateAsync(User user) =>
            await usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
