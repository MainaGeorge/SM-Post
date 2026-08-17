using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;

public class FindPostByAuthor : BaseQuery
{
    public string Author { get; set; }
}
