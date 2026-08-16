using Post.Common.Events;
using Post.Query.Domain.Entitites;
using Post.Query.Domain.Repository;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler(IPostRepository postRepository, ICommentRepository commentRepository) : IEventHandler
{
    public async Task On(PostCreatedEvent @event)
    {
        var post = new PostEntity
        {
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message,
            PostId = @event.Id
        };

        await postRepository.CreateAsync(post);
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);

        if (post is null) return;

        post.Message = @event.Message;
        await postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            Username = @event.Username,
            Edited = false,
        };

        await commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var comment = await commentRepository.GetByIdAsync(@event.CommentId);

        if(comment is null) return;

        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await commentRepository.UpdateAsync(comment);
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);

        if (post is null) return;
        post.Likes++;
        await postRepository.UpdateAsync(post);
    }

    public async Task On(PostRemovedEvent @event)
    {
        await postRepository.DeleteAsync(@event.Id);
    }

    public async Task On(CommentRemovedEvent @event)
    {
        await commentRepository.DeleteAsync(@event.CommentId);
    }
}
