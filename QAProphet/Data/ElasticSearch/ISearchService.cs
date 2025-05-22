namespace QAProphet.Data.ElasticSearch;

public interface ISearchService
{
    Task<bool> AddOrUpdateEntry(Guid id, string title, string? url, string type);
    Task<IndexEntry> SearchEntries(string startsWith, int pageSize, int pageNumber);
}