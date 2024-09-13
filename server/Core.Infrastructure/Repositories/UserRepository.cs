using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Npgsql;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Infrastructure.Snapshots;

namespace Core.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public UserRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT id, password_hash, first_name, second_name, birthdate, biography, city FROM users WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("id", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var userSnapshot = new UserSnapshot
                            {
                                Id = reader.GetString(0),
                                PasswordHash = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                SecondName = reader.GetString(3),
                                Birthdate = reader.GetDateTime(4),
                                Biography = reader.IsDBNull(5) ? null : reader.GetString(5),
                                City = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };

                            return _mapper.Map<User>(userSnapshot);
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<List<User>> SearchUsersAsync(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));

            var users = new List<User>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT id, password_hash, first_name, second_name, birthdate, biography, city 
                    FROM users 
                    WHERE first_name ILIKE @firstName AND second_name ILIKE @lastName";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("firstName", $"{firstName}%");
                    command.Parameters.AddWithValue("lastName", $"{lastName}%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var userSnapshot = new UserSnapshot
                            {
                                Id = reader.GetString(0),
                                PasswordHash = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                SecondName = reader.GetString(3),
                                Birthdate = reader.GetDateTime(4),
                                Biography = reader.IsDBNull(5) ? null : reader.GetString(5),
                                City = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };

                            users.Add(_mapper.Map<User>(userSnapshot));
                        }
                    }
                }
            }

            return users;
        }

        public async Task CreateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO users (id, password_hash, first_name, second_name, birthdate, biography, city) 
                    VALUES (@id, @passwordHash, @firstName, @secondName, @birthdate, @biography, @city)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("id", user.Id);
                    command.Parameters.AddWithValue("passwordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("firstName", user.FirstName);
                    command.Parameters.AddWithValue("secondName", user.SecondName);
                    command.Parameters.AddWithValue("birthdate", user.Birthdate);
                    command.Parameters.AddWithValue("biography", (object)user.Biography ?? DBNull.Value);
                    command.Parameters.AddWithValue("city", (object)user.City ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}