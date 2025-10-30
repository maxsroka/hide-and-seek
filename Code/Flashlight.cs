using Sandbox;

public class Flashlight : Component
{
    [RequireComponent]
    SpotLight SpotLight { get; set; }

	protected override void OnUpdate()
	{
		if (Input.Pressed("Flashlight"))
        {
            SpotLight.Enabled = !SpotLight.Enabled;
        }
	}
}