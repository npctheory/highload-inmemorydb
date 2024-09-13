using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Infrastructure.Snapshots;
using Npgsql;

namespace Core.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public PostRepository(string connectionString, IMapper mapper)
    {
        _connectionString = connectionString;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<Post>> ListPostsByUserId(string userId)
    {
        var posts = new List<Post>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM posts WHERE user_id = @userId ORDER BY created_at DESC", connection);
            command.Parameters.AddWithValue("@userId", userId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var postSnapshot = new PostSnapshot
                    {
                        Id = reader.GetGuid(0),
                        Text = reader.GetString(1),
                        UserId = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3)
                    };
                    posts.Add(_mapper.Map<Post>(postSnapshot));
                }
            }
        }
        return posts;
    }

    public async Task<List<Post>> ListPostsByUserIds(List<string> userIds, int offset, int limit)
    {
        var posts = new List<Post>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand(
                "SELECT * FROM posts WHERE user_id = ANY(@users) ORDER BY created_at DESC OFFSET @offset LIMIT @limit",
                connection);

            command.Parameters.AddWithValue("@users", userIds);
            command.Parameters.AddWithValue("@offset", offset);
            command.Parameters.AddWithValue("@limit", limit);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var post = new PostSnapshot
                    {
                        Id = reader.GetGuid(0),
                        Text = reader.GetString(1),
                        UserId = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3)
                    };

                    posts.Add(_mapper.Map<Post>(post));
                }
            }
        }
        return posts;
    }

    public async Task<Post> GetPostById(Guid postId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand("SELECT * FROM posts WHERE id = @postId", connection);
            command.Parameters.AddWithValue("@postId", postId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    var postSnapshot = new PostSnapshot
                    {
                        Id = reader.GetGuid(0),
                        Text = reader.GetString(1),
                        UserId = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3)
                    };

                    return _mapper.Map<Post>(postSnapshot);
                }
            }
        }
        return null;
    }

    public async Task<Post> CreatePost(string userId, string text)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand(
                "INSERT INTO posts (text, user_id) VALUES (@text, @userId) RETURNING id, text, user_id, created_at", connection);

            command.Parameters.AddWithValue("@text", text);
            command.Parameters.AddWithValue("@userId", userId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Post
                    {
                        Id = reader.GetGuid(0),
                        Text = reader.GetString(1),
                        UserId = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3)
                    };
                }
            }
        }
        return null;
    }

    public async Task<Post> UpdatePost(string userId, Guid postId, string text)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand(
                "UPDATE posts SET text = @text WHERE id = @postId AND user_id = @userId RETURNING id, text, user_id, created_at", connection);

            command.Parameters.AddWithValue("@text", text);
            command.Parameters.AddWithValue("@postId", postId);
            command.Parameters.AddWithValue("@userId", userId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Post
                    {
                        Id = reader.GetGuid(0),
                        Text = reader.GetString(1),
                        UserId = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3)
                    };
                }
            }
        }
        return null;
    }

    public async Task<Post> DeletePost(string userId, Guid postId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var post = await GetPostById(postId);

            if (post == null || post.UserId != userId)
                return null;

            var command = new NpgsqlCommand(
                "DELETE FROM posts WHERE id = @postId AND user_id = @userId", connection);

            command.Parameters.AddWithValue("@postId", postId);
            command.Parameters.AddWithValue("@userId", userId);

            var affectedRows = await command.ExecuteNonQueryAsync();

            return affectedRows > 0 ? post : null;
        }
    }
}
