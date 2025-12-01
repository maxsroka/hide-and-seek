using HNS;

public class Tracing : Component
{
	public SceneTraceResult Result { get; private set; }

	[ConVar("debug_tracing")]
	static bool IsDebugging { get; set; } = false;

	[ConVar("trace_distance", ConVarFlags.Replicated)]
	static float Distance { get; set; } = 100f;

	const float RADIUS = 10f;

	[RequireComponent]
	PlayerController Controller { get; set; }

	protected override void OnUpdate()
	{
		if (!Network.IsOwner) return;

		Result = Trace();

		if (IsDebugging)
		{
			DebugOverlay.Trace(Result);
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
