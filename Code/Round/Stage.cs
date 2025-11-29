using System;
using Sandbox;
namespace HNS;

public abstract class Stage : Component
{
	protected float Timer 
	{ 
		get => Round.Timer; 
		set => Round.Timer = value; 
	}

	public abstract float Duration { get; }
    
	[RequireComponent]
    protected Round Round { get; set; }
    
    public virtual void OnEnter() { }
    public virtual void OnRun() { }
    public virtual void OnExit() { }
	public virtual void OnJoin(Player player) { }
}
