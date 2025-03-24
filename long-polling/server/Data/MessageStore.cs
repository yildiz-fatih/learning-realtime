using LearningLongPolling.Models;

namespace LearningLongPolling.Data;

public delegate void MessageDelegate(Message message);

public class MessageStore
{
    private readonly List<Message> _messages = [];
    public event MessageDelegate? MessageAdded;

    public List<Message> GetAll()
    {
        return _messages.ToList();
    }

    public Message Create(Message message)
    {
        _messages.Add(message);

        if (MessageAdded != null)
        {
            MessageAdded.Invoke(message);
        }
        
        return message;
    }
}