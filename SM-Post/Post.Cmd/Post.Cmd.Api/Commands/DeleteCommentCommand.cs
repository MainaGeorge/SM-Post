using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands;

public class DeleteCommentCommand : BaseCommand
{
    public string Username { get; set; }
    public Guid CommentId { get; set; }
}
