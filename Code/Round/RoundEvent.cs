using Sandbox;

public interface IRoundEvent : ISceneEvent<IRoundEvent>
{
    void OnWaiting() { }
    void OnPreparing() { }
    void OnPlaying() { }
    void OnEnding() { }
}