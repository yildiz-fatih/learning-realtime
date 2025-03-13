using LearningLongPolling.Models;

namespace LearningLongPolling.Data;

public class MessageStore
{
    private readonly List<Message> _messages = [];

    public List<Message> GetAll()
    {
        return _messages.ToList();
    }

    public Message Create(Message message)
    {
        _messages.Add(message);
        
        return message;
    }
}