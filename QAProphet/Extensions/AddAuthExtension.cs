using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QAProphet.Options;

namespace QAProphet.Extensions;


internal  static class AddAuthExtension
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetRequiredSection(AuthOptions.Section).Get<AuthOptions>() ??
                          throw new Exception($"Missing {AuthOptions.Section} configuration section");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.RequireHttpsMetadata = false;
                o.MetadataAddress = authOptions.MetadataAddress;
                o.Audience = authOptions.ValidAudience;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authOptions.ValidIssuer,
                };
            });
        services.AddAuthorization();
        return services;
    }
}