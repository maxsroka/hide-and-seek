using Sandbox;

public sealed class Chat : Component, Component.INetworkListener
{
    /* Public Properties */

    public static Chat Instance => Game.ActiveScene.Get<Chat>();

    /* Public Methods */
    
    [Rpc.Broadcast]
    public void Say(string message)
    {
        GUI.Instance.ChatBox.AddMessage(new ChatBox.UserMessage(message, Rpc.Caller.DisplayName));
    }

    [Rpc.Broadcast(NetFlags.HostOnly)]
    public void Broadcast(string message)
    {
        GUI.Instance.ChatBox.AddMessage(new ChatBox.HostMessage(message));
    }

    /* Events */

    protected override void OnStart()
    {
        if (Connection.Local.IsHost)
        {
            BroadcastJoined(Connection.Host.DisplayName);
        }
    }

    /* Private Methods */

    void INetworkListener.OnConnected(Connection connection) => BroadcastJoined(connection.DisplayName);
    void INetworkListener.OnDisconnected(Connection connection) => BroadcastLeft(connection.DisplayName);
    
    void BroadcastJoined(string displayName) => Broadcast($"{displayName} has joined the game");
    void BroadcastLeft(string displayName) => Broadcast($"{displayName} has left the game");
}