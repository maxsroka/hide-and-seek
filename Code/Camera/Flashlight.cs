using Sandbox;
namespace HNS;

public class Flashlight : Component
{
    [RequireComponent]
    SpotLight SpotLight { get; set; }

	protected override void OnUpdate()
	{
		if (Input.Pressed("flashlight"))
        {
            SpotLight.Enabled = !SpotLight.Enabled;
        }
	}
}
