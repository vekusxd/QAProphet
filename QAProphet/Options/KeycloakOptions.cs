namespace QAProphet.Options;

public class KeycloakOptions
{
    public const string Section = "Keycloak";
    public required string AuthorizationUrl { get; set; }
}