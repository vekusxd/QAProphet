namespace QAProphet.Data.ElasticSearch;

public interface ISearchService
{
    Task<bool> RemoveEntry(Guid id);
    Task<bool> AddOrUpdateEntry(Guid id, string title, string? url, string type);
    Task<IReadOnlyCollection<IndexEntry>> SearchEntries(string startsWith, int pageSize, int pageNumber);
}