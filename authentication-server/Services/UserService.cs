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
        public async Task<User> GetAsync(string id) => 
            await usersCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
    }
}
