namespace Post.Cmd.Api.Commands;

public interface ICommandHandler
{
    Task HandleAsync(AddCommentCommand command);
    Task HandleAsync(DeleteCommentCommand command);
    Task HandleAsync(EditCommentCommand command);
    Task HandleAsync(EditMessageCommand command);
    Task HandleAsync(LikePostCommand command);
    Task HandleAsync(NewPostCommand command);
    Task HandleAsync(RemoveCommentCommand command);
    Task HandleAsync(DeletePostCommand command);
}
