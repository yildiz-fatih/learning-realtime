using LearningShortPolling.Data;
using LearningShortPolling.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningShortPolling.Controllers;

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
    public ActionResult<List<Message>> GetAll()
    {
        return GetMessagesInOrder(_store.GetAll());
        // return new Random().Next(0, 2) > 0.5 ? Ok(GetMessagesInOrder(_store.GetAll())) : (StatusCode(500, "An unexpected error occurred."));
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