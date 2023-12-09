using Hobron.SSE.Api.Dto;
using Hobron.SSE.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hobron.SSE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackChannelNotification : ControllerBase
    {
        private readonly ICustomMessageQueue _messageQueue;

        public BackChannelNotification(ICustomMessageQueue messageQueue)
        {
            this._messageQueue = messageQueue;
        }
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Notification notification)
        {
            try
            {
                _messageQueue.Register(notification.Id);
                await _messageQueue.EnqueueSync(notification, HttpContext.RequestAborted);
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
