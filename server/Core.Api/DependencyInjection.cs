using System.Text;
using Core.Api.EventConsumers;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Services;
using EventBus;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace Core.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();
                busConfigurator.AddConsumer<UserEventConsumer>();
                busConfigurator.AddConsumer<FriendEventConsumer>();
                busConfigurator.AddConsumer<PostEventConsumer>();

                busConfigurator.UsingRabbitMq((context, rabbitMqConfigurator) =>
                {
                    var rabbitMqSettings = configuration.GetSection("RabbitMqSettings");

                    rabbitMqConfigurator.Host(rabbitMqSettings["HostName"], "/", h =>
                    {
                        h.Username(rabbitMqSettings["UserName"]);
                        h.Password(rabbitMqSettings["Password"]);
                    });

                rabbitMqConfigurator.ReceiveEndpoint("core_queue", endpointConfigurator =>
                    {
                    endpointConfigurator.ConfigureConsumer<UserEventConsumer>(context);
                    endpointConfigurator.ConfigureConsumer<FriendEventConsumer>(context);
                    endpointConfigurator.ConfigureConsumer<PostEventConsumer>(context);
                    });
                });
            });
            services.AddTransient<IEventBus,RabbitMQEventBus>();

            services.AddSignalR();

            return services;
        }
    }
}