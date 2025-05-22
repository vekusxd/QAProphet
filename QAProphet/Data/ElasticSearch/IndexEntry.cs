namespace QAProphet.Data.ElasticSearch;

public class IndexEntry
{
    public const string IndexName = "index_entry";
    public Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Url { get; set; }
    public required string Type { get; init; }
}