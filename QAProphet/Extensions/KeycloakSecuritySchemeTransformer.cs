using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using QAProphet.Options;

namespace QAProphet.Extensions;

internal sealed class KeycloakSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IConfigurationManager _configurationManager;

    public KeycloakSecuritySchemeTransformer(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var keycloakOptions =
            _configurationManager.GetRequiredSection(KeycloakOptions.Section).Get<KeycloakOptions>() ??
            throw new Exception($"Missing {KeycloakOptions.Section} configuration section");

        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl =
                        new Uri(keycloakOptions.AuthorizationUrl),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "OpenID" },
                        { "profile", "Profile" }
                    },
                }
            },
            Name = "Bearer",
            In = ParameterLocation.Header,
            Scheme = "Bearer",
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        document.Components.SecuritySchemes["OAuth"] = securityScheme;

        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "OAuth",
                    Type = ReferenceType.SecurityScheme
                }
            }] = []
        };

        foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
        {
            operation.Value.Security.Add(requirement);
        }

        return Task.CompletedTask;
    }
}