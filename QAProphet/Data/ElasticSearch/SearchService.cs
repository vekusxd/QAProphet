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

    public async Task<bool> RemoveEntry(Guid id)
    {
        var result =
            await _client.DeleteAsync<IndexEntry>(id, configure => configure.Index(IndexEntry.IndexName));

        if (!result.IsValidResponse)
        {
            _logger.LogError("Failed to delete index entry with id: {@id} {@error}",
                id, result.ElasticsearchServerError);
            return false;
        }

        return true;
    }

    public async Task<bool> AddOrUpdateEntry(Guid id, string title, string? url, string type)
    {
        var indexEntry = new IndexEntry
        {
            Id = id,
            Title = title,
            Url = url ?? throw new ArgumentNullException(nameof(url)),
            Type = type
        };

        var upsertResult = await _client.UpdateAsync<IndexEntry, IndexEntry>(IndexEntry.IndexName, id,
            configure =>
            {
                configure.Upsert(indexEntry);
                configure.Doc(indexEntry);
            });

        if (!upsertResult.IsValidResponse)
        {
            _logger.LogError("Failed to upsert index entry with id: {@indexEntryId}, {@indexEntryTitle}, {@error}", id,
                title, upsertResult.ElasticsearchServerError);
            return false;
        }

        return true;
    }

    public async Task<IReadOnlyCollection<IndexEntry>> SearchEntries(string startsWith, int pageSize, int pageNumber)
    {
        var offset = pageSize * (pageNumber - 1);

        var response = await _client.SearchAsync<IndexEntry>(e => e
            .Indices(IndexEntry.IndexName)
            .From(offset)
            .Size(pageSize)
            .Query(q => q.Prefix(w => w
                .Field(f => f.Title)
                .Value(startsWith)
                .CaseInsensitive()))
            .Sort(so => so.Field(f => f.Type, SortOrder.Desc)));

        return response.Documents;
    }
}