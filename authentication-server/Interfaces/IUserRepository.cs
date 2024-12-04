using authentication_server.Models;

namespace authentication_server.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetByIdAsync(string id);
        public Task<User> GetByEmailAsync(string email);
        public Task CreateAsync(User user);
        public Task UpdateAsync(User user);
    }
}
