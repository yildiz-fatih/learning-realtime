namespace LearningShortPolling.Models;

public class Message
{
    public required string Username { get; set; }
    public required string Text { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
}