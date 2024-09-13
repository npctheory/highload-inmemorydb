using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Npgsql;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Infrastructure.Snapshots;

namespace Core.Infrastructure.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public FriendshipRepository(string connectionString, IMapper mapper)
    {
        _connectionString = connectionString;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task AddFriendship(string userId, string friendId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("INSERT INTO friendships (user_id, friend_id) VALUES (@UserId, @FriendId)", connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@FriendId", friendId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task DeleteFriendship(string userId, string friendId)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("DELETE FROM friendships WHERE user_id = @UserId AND friend_id = @FriendId", connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@FriendId", friendId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<List<Friendship>> ListFriendships(string userId)
    {
        var friendships = new List<Friendship>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("SELECT friend_id FROM friendships WHERE user_id = @UserId", connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var friendshipSnapshot = new FriendshipSnapshot
                        {
                            UserId = userId,
                            FriendId = reader.GetString(0)
                        };

                        friendships.Add(_mapper.Map<Friendship>(friendshipSnapshot));
                    }
                }
            }
        }

        return friendships;
    }

    public async Task<List<Friendship>> ListUsersWithFriend(string friendId)
    {
        var friendships = new List<Friendship>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new NpgsqlCommand("SELECT user_id FROM friendships WHERE friend_id = @FriendId", connection))
            {
                command.Parameters.AddWithValue("@FriendId", friendId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        friendships.Add(new Friendship
                        {
                            UserId = reader.GetString(0),
                            FriendId = friendId
                        });
                    }
                }
            }
        }

        return friendships;
    }
}
