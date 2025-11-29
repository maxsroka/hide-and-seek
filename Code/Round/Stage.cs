using System;
using Sandbox;
namespace HNS;

public abstract class Stage : Component
{
	public abstract float Duration { get; }
    
    public virtual void OnEnter() { }
    public virtual void OnRun() { }
    public virtual void OnExit() { }
	public virtual void OnJoin(Player player) { }
	
	[RequireComponent]
    protected Round Round { get; set; }
}
