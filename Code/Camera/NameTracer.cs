using Sandbox;
namespace HNS;

public class NameTracer : Component
{
	public interface ITraceListener : ISceneEvent<ITraceListener>
	{
		void OnStart(Player player) { }
		void OnEnd() { }
	}

	bool traced = false;

	protected override void OnFixedUpdate()
	{
		var trace = Trace();
		var player = GetPlayer(trace);

		if (player != null && !traced)
		{
			traced = true;
			ITraceListener.Post(e => e.OnStart(player));
		}
		else if (player == null && traced)
		{
			traced = false;
			ITraceListener.Post(e => e.OnEnd());
		}
	}

	SceneTraceResult Trace()
	{
		var ray = Transform.World.ForwardRay;
		var length = 300f;
		var trace = Scene.Trace.Ray(ray, length);

		var localPlayer = Player.GetLocal();
		if (localPlayer != null)
		{
			trace = trace.IgnoreGameObject(localPlayer.GameObject);
		}

		return trace.Run();
	}

	Player GetPlayer(SceneTraceResult trace)
	{
		if (!trace.Hit) return null;
		
		return trace.GameObject.GetComponent<Player>();
	}
}
