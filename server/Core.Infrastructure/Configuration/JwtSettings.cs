namespace Core.Infrastructure.Configuration;

public class JwtSettings
{
    public string Secret {get; init;} = null!;
    public int ExpirationTimeInMinutes {get;init;}
    public string Issuer {get;init;} = null!;
    public string Audience{get;init;} = null!;
}