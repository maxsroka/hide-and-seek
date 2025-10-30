using Sandbox;

public interface IChatEvents : ISceneEvent<IChatEvents>
{
    void OnUserMessage(string message, string sender) { }
    void OnSystemMessage(string message) { }
}

public sealed class Chat : Component, Component.INetworkListener
{
    public static Chat Instance => Game.ActiveScene.Get<Chat>();

    [Rpc.Broadcast]
    public void UserMessage(string message)
    {
        IChatEvents.Post(e => e.OnUserMessage(message, Rpc.Caller.DisplayName));
    }

    [Rpc.Broadcast(NetFlags.HostOnly)]
    public void SystemMessage(string message)
    {
        IChatEvents.Post(e => e.OnSystemMessage(message));
    }

    protected override void OnStart()
    {
        if (Connection.Local.IsHost)
        {
            JoinedSystemMessage(Connection.Host.DisplayName);
        }
    }

    void INetworkListener.OnConnected(Connection connection) => JoinedSystemMessage(connection.DisplayName);
    void INetworkListener.OnDisconnected(Connection connection) => LeftSystemMessage(connection.DisplayName);

    void JoinedSystemMessage(string displayName) => SystemMessage($"{displayName} has joined the game");
    void LeftSystemMessage(string displayName) => SystemMessage($"{displayName} has left the game");
}