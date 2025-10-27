using System;
using Sandbox.UI;

public sealed class GUI : PanelComponent
{
    HUD hud;
    Blindness blindness;

    protected override void OnTreeFirstBuilt()
    {
        Panel.Id = "gui";
        blindness = new() { Parent = Panel, Id = "blindness" };
        hud = new() { Parent = Panel, Id = "hud" };
    }

    protected override void OnUpdate()
    {
        var round = Round.Instance;
        hud.SetTime(round.GetTimeRemaining());

        if (round.Stage == RoundStage.Waiting)
        {
            hud.SetClass("hider", false);
            hud.SetClass("seeker", false);
            hud.SetClass("waiting", true);
            hud.SetStatus("Waiting for players...");
        }
        else
        {
            hud.SetClass("waiting", false);

            var localPlayer = Player.GetLocal();
            if (localPlayer.Role == Role.Hider)
            {
                hud.SetClass("seeker", false);
                hud.SetClass("hider", true);
                hud.SetStatus("Hider");
            }
            else if (localPlayer.Role == Role.Seeker)
            {
                hud.SetClass("hider", false);
                hud.SetClass("seeker", true);
                hud.SetStatus("Seeker");
            }
        }

        blindness.Toggle(Player.GetLocal().IsBlinded);
    }

    [StyleSheet("GUI.scss")]
    class HUD : Panel
    {
        Panel box;
        Panel bar;
        Label status;
        Label time;

        public HUD()
        {
            box = new() { Parent = this, Id = "box" };
            bar = new() { Parent = box, Id = "bar" };
            status = new("Waiting") { Parent = bar, Id = "status" };
            time = new("00:00") { Parent = box, Id = "time" };
        }

        public void SetStatus(string text)
        {
            status.Text = text;
        }

        public void SetTime(float seconds)
        {
            time.Text = TimeSpan.FromSeconds(seconds).ToString("mm\\:ss");
        }
    }

    [StyleSheet("GUI.scss")]
    class Blindness : Panel
    {
        public void Toggle(bool toggle)
        {
            SetClass("visible", toggle);
        }
    }
}