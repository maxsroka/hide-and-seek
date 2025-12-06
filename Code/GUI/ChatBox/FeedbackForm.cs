using Sandbox;
using Sandbox.Services;
namespace HNS;

public class FeedbackForm : Component, Chat.IMessageListener
{
	const string SECRET_CODE = "crux";

	void Chat.IMessageListener.OnUserMessage(string message, Connection sender)
	{
		if (Connection.Local != sender) return;

		if (message.ToLower() == SECRET_CODE)
		{
			Achievements.Unlock("form_code");
		}
	}
}
