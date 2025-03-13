using LearningLongPolling.Data;
using LearningLongPolling.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningLongPolling.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly MessageStore _store;

    public MessagesController(MessageStore store)
    {
        _store = store;
    }

    [HttpGet]
    public async Task<ActionResult<List<Message>>> GetAll([FromQuery] DateTime? lastReceived)
    {
        var startTime = DateTime.UtcNow;
        var checkInterval = TimeSpan.FromMilliseconds(500);
        var timeOut = TimeSpan.FromSeconds(30);

        while (DateTime.UtcNow - startTime < timeOut)
        {
            if ((lastReceived ?? DateTime.MinValue) < _store.GetAll().Last().Time)
            {
                return GetMessagesInOrder(_store.GetAll());
                // return new Random().Next(0, 2) > 0.5 ? Ok(GetMessagesInOrder(_store.GetAll())) : (StatusCode(500, "An unexpected error occurred."));
            }
            
            await Task.Delay(checkInterval);
        }

        return NoContent();
    }

    private List<Message> GetMessagesInOrder(List<Message> messages)
    {
        return messages.AsEnumerable().Reverse().ToList();
    }

    [HttpPost]
    public ActionResult Create([FromBody] Message message)
    {
        var msg = _store.Create(message);
        return Ok(msg);
    }
}