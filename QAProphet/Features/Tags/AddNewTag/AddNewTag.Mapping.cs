using QAProphet.Domain;

namespace QAProphet.Features.Tags.AddNewTag;

internal static class AddTagMappingExtensions
{
    public static AddTagResponse MapToCreateResponse(this Tag tag)
    => new(tag.Id, tag.Title, tag.Description);
    
    public static AddTagCommand MapToCommand(this AddTagRequest request)
    => new(request.Title, request.Description);
}