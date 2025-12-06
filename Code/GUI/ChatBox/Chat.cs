using Sandbox;
using System;
namespace HNS;

public static class Chat
{
	public interface IMessageListener : ISceneEvent<IMessageListener>
	{
		void OnUserMessage(string message, Connection sender) { }
		void OnSystemMessage(string message) { }
	}

    [Rpc.Host]
    public static void UserMessage(string message)
    {
		if (!ValidateUserMessage(message)) return;

		BroadcastUserMessage(message, Rpc.Caller.Id);
	}

	static bool ValidateUserMessage(string message)
	{
		if (message.Length > 128) return false;

		return true;
	}

	[Rpc.Broadcast(NetFlags.HostOnly)]
	static void BroadcastUserMessage(string message, Guid senderId)
	{
		var sender = Connection.Find(senderId);
		if (sender == null) return;

		IMessageListener.Post(e => e.OnUserMessage(message, sender));
	}

	[Rpc.Broadcast(NetFlags.HostOnly)]
    public static void SystemMessage(string message)
    {
		IMessageListener.Post(e => e.OnSystemMessage(message));
	}
}
