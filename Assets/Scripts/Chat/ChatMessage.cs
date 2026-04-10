using System;

[Serializable]
public class ChatMessage
{
    public string senderName;
    public string content;
    public bool isPrivate;
    public string timestamp;
}