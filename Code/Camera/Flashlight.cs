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
			PlaySound(SpotLight.Enabled);
		}
	}

	void PlaySound(bool enabled)
	{
		Sound.Play(enabled ? "flashlight_on" : "flashlight_off");
	}
}
