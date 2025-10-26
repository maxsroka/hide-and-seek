using Sandbox;

public sealed class PlayerFinder : Component
{
    public static IEnumerable<GameObject> All => Game.ActiveScene.GetAllComponents<PlayerFinder>().Select(f => f.GameObject); 
}