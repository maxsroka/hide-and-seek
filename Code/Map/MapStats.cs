using Sandbox;
using Sandbox.Services;
namespace HNS;

public class MapStats : Component
{
	[RequireComponent]
	MapInstance MapInstance { get; set; }

	protected override void OnStart()
	{
		Stats.Increment($"map-loaded-({MapInstance.MapName})", 1);
	}
}
