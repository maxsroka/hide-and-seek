using System;
using Sandbox;

public sealed class PlayerFinder : Component
{
    public static IEnumerable<GameObject> All => Game.ActiveScene.GetAllComponents<PlayerFinder>().Select(f => f.GameObject);

    public static GameObject OwnedBy(Connection connection)
    {
        var components = Game.ActiveScene.GetAllComponents<PlayerFinder>();
        var first = components.First(f => f.Network.OwnerId == connection.Id);
        return first.GameObject; 
    } 
}