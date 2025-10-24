using Sandbox;
using Sandbox.UI;

public sealed class RootPanel : PanelComponent
{
    StatusPanel statusPanel;

    protected override void OnTreeFirstBuilt()
    {
        base.OnTreeFirstBuilt();

        statusPanel = new();
        statusPanel.Parent = Panel;
    }

    class StatusPanel : Panel
    {
        public Label label;

        public StatusPanel()
        {
            label = new Label( "test" );
            label.Parent = this;
        }
    }
}