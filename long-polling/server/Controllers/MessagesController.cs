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
        // If there is a new message, return it immediately
        var messages = _store.GetAll();
        var lastTime = lastReceived ?? DateTime.MinValue;

        if (messages.Last().Time > lastTime)
        {
            return Ok(GetMessagesInOrder(messages));
        }
        
        // TaskCompletionSource will complete when a new message is added
        var tcs = new TaskCompletionSource<bool>();
        
        // This event handler will be called when a new message is added to the store
        void OnMessageAdded(Message newMessage)
        {
            if (newMessage.Time > lastTime)
            {
                tcs.TrySetResult(true);
            }
        }

        try
        {
            // Subscribe to the MessageAdded event
            _store.MessageAdded += OnMessageAdded;
            
            // We asynchronously wait here until:
            //   -> a new message arrives and triggers OnMessageAdded (and therefore completes the task), or
            //   -> 30 seconds pass, whichever happens first
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            // In case of timeout, return 204 NoContent
            if (completedTask == timeoutTask)
            {
                return NoContent();
            }

            // In case a new message arrives within 30 seconds
            messages = _store.GetAll();
            return Ok(GetMessagesInOrder(messages));
        }
        finally
        {
            // Unsubscribe from the event
            _store.MessageAdded -= OnMessageAdded;
        }
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