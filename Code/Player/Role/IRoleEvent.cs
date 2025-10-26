using Sandbox;

public interface IRoleEvent : ISceneEvent<IRoleEvent>
{
    void OnHider() { }
    void OnSeeker() { }
}