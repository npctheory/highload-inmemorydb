using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Domain.Aggregates;
using AutoMapper;

namespace Core.Infrastructure.Repositories
{
    public class CitusDialogMessageRepository : IDialogRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public CitusDialogMessageRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<DialogMessage>> ListMessages(string userId, string agentId)
        {
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (agentId == null) throw new ArgumentNullException(nameof(agentId));

            var messages = new List<DialogMessage>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queries = new[]
                {
                    @"
                        SELECT id, sender_id, receiver_id, text, is_read, timestamp
                        FROM dialog_messages_sent
                        WHERE sender_id = @userId AND receiver_id = @agentId",
                    @"
                        SELECT id, sender_id, receiver_id, text, is_read, timestamp
                        FROM dialog_messages_received
                        WHERE sender_id = @agentId AND receiver_id = @userId"
                };

                foreach (var query in queries)
                {
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("userId", userId);
                        command.Parameters.AddWithValue("agentId", agentId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var message = new DialogMessage
                                {
                                    Id = reader.GetGuid(0),
                                    SenderId = reader.GetString(1),
                                    ReceiverId = reader.GetString(2),
                                    Text = reader.GetString(3),
                                    IsRead = reader.GetBoolean(4),
                                    Timestamp = reader.GetDateTime(5)
                                };

                                messages.Add(message);
                            }
                        }
                    }
                }
            }

            return messages;
        }

    public async Task<DialogMessage> SendMessage(DialogMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        var messageId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;

        message.Id = messageId;
        message.Timestamp = timestamp;

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    var insertQueries = new[]
                    {
                        @"
                            INSERT INTO dialog_messages_sent (id, sender_id, receiver_id, text, is_read, timestamp)
                            VALUES (@id, @senderId, @receiverId, @text, @isRead, @timestamp)",
                        @"
                            INSERT INTO dialog_messages_received (id, sender_id, receiver_id, text, is_read, timestamp)
                            VALUES (@id, @senderId, @receiverId, @text, @isRead, @timestamp)"
                    };

                    foreach (var query in insertQueries)
                    {
                        using (var command = new NpgsqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("id", messageId);
                            command.Parameters.AddWithValue("senderId", message.SenderId);
                            command.Parameters.AddWithValue("receiverId", message.ReceiverId);
                            command.Parameters.AddWithValue("text", message.Text);
                            command.Parameters.AddWithValue("isRead", message.IsRead);
                            command.Parameters.AddWithValue("timestamp", timestamp);

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    await transaction.CommitAsync();

                    return message;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }

        public async Task<List<Dialog>> ListDialogs(string userId)
        {
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            var dialogs = new List<Dialog>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT DISTINCT sender_id AS agent_id
                    FROM dialog_messages_sent
                    WHERE receiver_id = @userId
                    UNION
                    SELECT DISTINCT receiver_id AS agent_id
                    FROM dialog_messages_received
                    WHERE sender_id = @userId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("userId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dialogs.Add(new Dialog
                            {
                                AgentId = reader.GetString(0)
                            });
                        }
                    }
                }
            }

            return dialogs;
        }
    }
}
