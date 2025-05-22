using Elastic.Clients.Elasticsearch;

namespace QAProphet.Data.ElasticSearch;

public class SearchService : ISearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<SearchService> _logger;

    public SearchService(ElasticsearchClient client, ILogger<SearchService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<bool> IndexEntry(Guid id, string title, string? url, string type)
    {
        var indexEntry = new IndexEntry
        {
            Id = id,
            Title = title,
            Url = url ?? throw new ArgumentNullException(nameof(url)),
            Type = type
        };

        var upsertResult = await _client.UpdateAsync<IndexEntry, IndexEntry>(ElasticSearch.IndexEntry.IndexName, id,
            configure =>
            {
                configure.Upsert(indexEntry);
                configure.Doc(indexEntry);
            });

        if (!upsertResult.IsValidResponse)
        {
            _logger.LogError("Failed to upsert index entry with id: {@indexEntryId}, {@indexEntryTitle}, {@error}", id, title, upsertResult.ElasticsearchServerError);
            return false;
        }

        return true;
    }
}