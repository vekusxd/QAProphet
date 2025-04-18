namespace QAProphet.Options;

public class AuthOptions
{
    public const string Section = "Authentication";
    public required string MetadataAddress { get; set; }
    public required string ValidIssuer { get; set; }
    public required string ValidAudience { get; set; }
}