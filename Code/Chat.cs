using Sandbox;

public sealed class Chat : Component
{
    public static Chat Instance => Game.ActiveScene.Get<Chat>();

    [RequireComponent]
    GUI GUI { get; set; }

    [Rpc.Broadcast]
    public void Send(string message)
    {
        GUI.ChatBox.AddMessage(new ChatBox.Message() { content = message, sender = Rpc.Caller.DisplayName });
    }
}