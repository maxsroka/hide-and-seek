using Sandbox;
namespace HNS;

public static class Chat
{
	public interface IMessageListener : ISceneEvent<IMessageListener>
	{
		void OnUserMessage(string message, Connection sender) { }
		void OnSystemMessage(string message) { }
	}

    [Rpc.Broadcast]
    public static void UserMessage(string message)
    {
		#if !SERVER
		IMessageListener.Post(e => e.OnUserMessage(message, Rpc.Caller));
		#endif
	}

    [Rpc.Broadcast(NetFlags.HostOnly)]
    public static void SystemMessage(string message)
    {
		#if !SERVER
		IMessageListener.Post(e => e.OnSystemMessage(message));
		#endif
	}
}
