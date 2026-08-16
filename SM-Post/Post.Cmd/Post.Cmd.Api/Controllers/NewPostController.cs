using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;

namespace Post.Cmd.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
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
}
