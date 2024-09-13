using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces;
public interface IPostRepository
{
    Task<List<Post>> ListPostsByUserId(string userId);
    Task<List<Post>> ListPostsByUserIds(List<string> userIds, int offset, int limit);
    Task<Post> GetPostById(Guid postId);
    Task<Post> DeletePost(string userId, Guid postId);
    Task<Post> CreatePost(string userId, string text);
    Task<Post> UpdatePost(string userId, Guid postId, string text);
}
