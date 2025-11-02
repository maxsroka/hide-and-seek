using System;
using Sandbox;
namespace Round;

public abstract class Stage : Component
{
    [RequireComponent]
    protected Round Round { get; set; }
    
    public virtual void OnEnter() { }
    public virtual void OnRun() { }
    public virtual void OnExit() { }
    public virtual void OnPlayerJoined(Connection connection) { }
    public virtual void OnPlayerLeft(Connection connection) { }
}