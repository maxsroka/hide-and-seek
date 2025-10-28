using Sandbox;

public sealed class Chat : Component
{
    public static Chat Instance => Game.ActiveScene.Get<Chat>();

    [RequireComponent]
    GUI GUI { get; set; }

    [Rpc.Broadcast]
    public void Say(string message)
    {
        GUI.ChatBox.AddMessage(new ChatBox.UserMessage(message, Rpc.Caller.DisplayName));
    }

    [Rpc.Broadcast(NetFlags.HostOnly)]
    public void Broadcast(string message)
    {
        GUI.ChatBox.AddMessage(new ChatBox.HostMessage(message));
    }
}