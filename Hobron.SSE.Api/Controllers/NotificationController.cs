using Hobron.SSE.Api.Dto;
using Hobron.SSE.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Hobron.SSE.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ICustomMessageQueue _messageQueue;
        public NotificationController(ICustomMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        [HttpPost("Subscribe/{id}")]
        public async Task<IActionResult> Subscribe(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            this.Response.StatusCode = 200;
            this.Response.Headers.Add("Content-Type", "text/event-stream");
            this.Response.Headers.Add("Cache-Control", "no-cache");
            this.Response.Headers.Add("Connection", "Keep-alive");
            try
            {
                var notification = new Notification { Id = id, Message = $"Subscribed to client {id}" };
                _messageQueue.Register(id);
                await _messageQueue.EnqueueSync(notification, HttpContext.RequestAborted);

                var streamWriter = new StreamWriter(this.Response.Body);
                await foreach (var message in _messageQueue.DequeueAsync(id, HttpContext.RequestAborted))
                {
                    await streamWriter.WriteLineAsync($"Message received: {message} at {DateTime.Now}");
                    await streamWriter.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                _messageQueue.Unregister(id);
            }
            return Ok();
        }

        [HttpGet("Message/{id}")]
        public async Task<IActionResult> GetMessage(string id)
        {
            this.Response.StatusCode = 200;
            this.Response.Headers.Add("Content-Type", "text/event-stream");
            this.Response.Headers.Add("Cachce-Control", "no-cache");
            this.Response.Headers.Add("Connection", "keep-alive");
            try
            {
                var streamWriter = new StreamWriter(this.Response.Body);
                await foreach (var message in _messageQueue.DequeueAsync(id, this.HttpContext.RequestAborted))
                {
                    await streamWriter.WriteLineAsync($"{DateTime.Now} - {message}");
                    await streamWriter.FlushAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Message")]
        public async Task<IActionResult> GetMessages()
        {
            this.Response.StatusCode = 200;
            this.Response.Headers.Add("Content-Type", "text/event-stream");
            this.Response.Headers.Add("Cache-Control", "no-cache");
            this.Response.Headers.Add("Connection", "keep-alive");

            try
            {
                var streamWriter = new StreamWriter(this.Response.Body);
                await foreach(var message in _messageQueue.DequeueAsync(this.HttpContext.RequestAborted))
                {
                    await streamWriter.WriteLineAsync($"{DateTime.Now} - {message}");
                    await streamWriter.FlushAsync();
                }
                return Ok();
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
