using MediaService.Contract.Enumarations.Post;
using MongoDB.Bson;

namespace MediaService.Contract.Services.Category;
public static class Filter
{
    public record CategoryFilter(string? SearchContent);
}
