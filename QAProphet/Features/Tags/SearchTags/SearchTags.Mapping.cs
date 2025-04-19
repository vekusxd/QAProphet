using QAProphet.Domain;
using QAProphet.Features.Shared.Responses;

namespace QAProphet.Features.Tags.SearchTags;

internal static class SearchTagsMappingExtensions
{
    public static TagResponse MapToResponse(this Tag tag)
        => new(tag.Id, tag.Title);
}