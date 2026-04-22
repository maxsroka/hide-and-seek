using System;
using Sandbox;
namespace HNS;

public abstract class Stage : Component
{
	public abstract float Duration { get; }

	public abstract string GetDescription();

    public virtual void OnEnter() { }
    
	public virtual void OnRun() 
	{
		TryPlayTimerSound();
	}

    public virtual void OnExit() { }
	public virtual void OnJoin(Player player) { }
	
	[RequireComponent]
    protected Round Round { get; set; }

	bool DidPlayTimerSound { get; set; }

	void TryPlayTimerSound()
	{
		if (!DidPlayTimerSound && Round.TimeLeft <= 5f)
		{
			Round.PlayTimerSound();
			DidPlayTimerSound = true;
		}
	}
}
