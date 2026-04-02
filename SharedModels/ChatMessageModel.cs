namespace SharedModels;

public class ChatMessageModel
{
    public string Message { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public ChatSendAndReceiveType Type { get; set; }
}
public enum ChatSendAndReceiveType : int
{
    None = 0,
    UserInput = 1,
    AssistantInput = 2,
}
