using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[Route("api/v1/posts")]
[ApiController]
public class PostsController(ILogger<PostsController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreatePostAsync(NewPostCommand command)
    {
        try
        {
            var id = Guid.NewGuid();
            command.Id = id;
            await commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status201Created, new NewPostResponse { Id = id, Message = "New post creation request completed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return BadRequest(new NewPostResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post!";
            logger.LogError(ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> UpdatePostAsync(EditMessageCommand command, Guid id)
    {
        try
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse { Message = $"post updated successfully!" });
        }
        catch (AggregateNotFoundException ex)
        {
            logger.LogWarning(ex, "Client provide an invalid post ID!");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to update a new post!";
            logger.LogError(ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }

    [HttpPut]
    [Route("{id}/like")]
    public async Task<ActionResult> LikePostAsync(LikePostCommand command, Guid id)
    {
        try
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse { Message = $"post liked successfully!" });
        }
        catch (AggregateNotFoundException ex)
        {
            logger.LogWarning(ex, "Client provide an invalid post ID!");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while trying to like a post!";
            logger.LogError(ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }

    [HttpPost]
    [Route("{id}/comment")]
    public async Task<ActionResult> AddCommentAsync(AddCommentCommand command, Guid id)
    {
        try
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse { Message = $"comment added successfully!" });
        }
        catch (AggregateNotFoundException ex)
        {
            logger.LogWarning(ex, "Client provide an invalid post ID!");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while trying to add a comment to a post!";
            logger.LogError(ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }

    [HttpPut]
    [Route("{id}/comments")]
    public async Task<ActionResult> UpdateCommentAsync(EditCommentCommand command, Guid id)
    {
        try
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse { Message = $"comment updated successfully!" });
        }
        catch (AggregateNotFoundException ex)
        {
            logger.LogWarning(ex, "Client provide an invalid post ID!");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while trying to update a comment to a post!";
            logger.LogError(ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }

    [HttpDelete]
    [Route("{id}/comments")]
    public async Task<ActionResult> DeleteCommentAsync(DeleteCommentCommand command, Guid id)
    {
        try
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse { Message = $"comment deleted successfully!" });
        }
        catch (AggregateNotFoundException ex)
        {
            logger.LogWarning(ex, "Client provide an invalid post ID!");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while trying to delete a comment from a post!";
            logger.LogError(ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeletePostAsync(DeletePostCommand command, Guid id)
    {
        try
        {
            command.Id = id;
            await commandDispatcher.SendAsync(command);

            return StatusCode(StatusCodes.Status200OK, new BaseResponse { Message = $"comment deleted successfully!" });
        }
        catch (AggregateNotFoundException ex)
        {
            logger.LogWarning(ex, "Client provide an invalid post ID!");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Client made a bad request");
            return BadRequest(new BaseResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while trying to delete a comment from a post!";
            logger.LogError(ex, SAFE_ERROR_MESSAGE);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR_MESSAGE });
        }
    }
}
