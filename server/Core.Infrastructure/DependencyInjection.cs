using Core.Domain.Interfaces;
using Core.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Core.Infrastructure.Mapping;
using StackExchange.Redis;
using Core.Application.Abstractions;
using Core.Application.Services;
using Core.Infrastructure.Services;
using MassTransit;
using Core.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Core.Infrastructure.Generators;
using System.Text;
using Core.Infrastructure.Providers;
using EventBus;
using EventBus.Events;


namespace Core.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDateTimeProvider,DateTimeProvider>();
            services.AddSingleton<IJwtTokenGenerator,JwtTokenGenerator>();
            services.AddSingleton<IPasswordHasher,PasswordHasher>();


            services.AddAutoMapper(cfg => 
            {
                cfg.AddProfile<InfrastructureProfile>();
            }, typeof(DependencyInjection).Assembly);

            var redisConnectionString = configuration.GetSection("RedisSettings:ConnectionString").Value;
            services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddSingleton<ICacheService>(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return new RedisCacheService(redis, databaseIndex: 0);
            });

            services.AddScoped<IUserRepository>(sp =>
            {
                var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
                var mapper = sp.GetRequiredService<IMapper>();
                return new UserRepository(connectionString, mapper);
            });

            services.AddScoped<IFriendshipRepository>(sp =>
            {
                var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
                var mapper = sp.GetRequiredService<IMapper>();
                return new FriendshipRepository(connectionString, mapper);
            });

            services.AddScoped<IPostRepository>(sp =>
            {
                var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
                var mapper = sp.GetRequiredService<IMapper>();
                return new PostRepository(connectionString, mapper);
            });

            var repositoryType = configuration.GetSection("DialogRepositorySettings:Type").Value;

            if (repositoryType == "Redis")
            {
                services.AddScoped<IDialogRepository>(sp =>
                {
                    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                    return new RedisDialogRepository(redis, databaseIndex: 1);
                });
            }
            else if (repositoryType == "Postgres")
            {
                services.AddScoped<IDialogRepository>(sp =>
                {
                    var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
                    var mapper = sp.GetRequiredService<IMapper>();
                    return new PostgresDialogRepository(connectionString, mapper);
                });
            }
            else
            {
                throw new Exception("Invalid repository type specified in configuration.");
            }


            return services;
        }
    }
}