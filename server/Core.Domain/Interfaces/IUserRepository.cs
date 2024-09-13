using Core.Domain.Entities;
using System.Threading.Tasks;

namespace Core.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<List<User>> SearchUsersAsync(string firstName, string lastName);
        Task CreateUserAsync(User user);
    }
}