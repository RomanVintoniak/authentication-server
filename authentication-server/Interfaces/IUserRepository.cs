using authentication_server.Models;

namespace authentication_server.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetAsync(string id);
    }
}
