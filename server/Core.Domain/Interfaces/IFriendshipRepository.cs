using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IFriendshipRepository
{
    Task AddFriendship(string userId, string friendId);
    Task DeleteFriendship(string userId, string friendId);
    Task<List<Friendship>> ListFriendships(string userId);
    Task<List<Friendship>> ListUsersWithFriend(string friendId);
}

