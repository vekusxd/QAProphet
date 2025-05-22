namespace QAProphet.Data.ElasticSearch;

public interface ISearchService
{
    Task<bool> IndexEntry(Guid id, string title, string? url, string type);
}