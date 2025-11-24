using Sandbox;
namespace HNS;

public class Pushing : Component
{
	[ConVar("debug_pushing")]
	static bool Debug { get; set; } = false;

	[ConVar("push_strength", ConVarFlags.Replicated)]
	static float Strength { get; set; } = 500f;

	[ConVar("push_distance", ConVarFlags.Replicated)]
	static float Distance { get; set; } = 100f;
	
	const float RADIUS = 10;

	[RequireComponent]
	PlayerController Controller { get; set; }

	[Rpc.Owner]
	void Push(Vector3 direction)
	{
		Controller.Jump(direction * Strength);
	}

	protected override void OnUpdate()
	{
		if (!Network.IsOwner) return;

		bool input = Input.Pressed("use");

		if (input || Debug)
		{
			var trace = Trace();

			if (input)
			{
				if (trace.Hit && trace.GameObject.Components.TryGet(out Pushing pushing))
				{
					pushing.Push(trace.Direction);
				}
			}

			if (Debug)
			{
				DebugOverlay.Trace(trace);
			}
		}
	}

	SceneTraceResult Trace()
	{
		Vector3 origin;
		if (!Controller.ThirdPerson)
		{
			origin = Scene.Camera.WorldPosition;
		}
		else
		{
			origin = GameObject.WorldPosition + Vector3.Up * Controller.BodyHeight;
		}

		var direction = Scene.Camera.WorldTransform.Forward;
		var ray = new Ray(origin, direction);
		var trace = Scene.Trace.Sphere(RADIUS, ray, Distance);
		trace = trace.IgnoreGameObject(GameObject);
		
		return trace.Run();
	}
}
