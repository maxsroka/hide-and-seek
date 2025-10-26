using Sandbox;

public sealed class PreparingStage : Component, IRoundEvent
{
    void IRoundEvent.OnPreparing()
    {
        Log.HideAndSeek.Info("Preparing has started!");

        var spawnPoint = GetRandomSpawnPoint();
        foreach (var player in PlayerFinder.All)
        {
            var utils = player.GetComponent<PlayerControllerUtils>();
            utils.Teleport(spawnPoint.WorldPosition);
        }
    }

    GameObject GetRandomSpawnPoint()
    {
        var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToArray();
        var spawnPoint = Game.Random.FromArray(spawnPoints);

        return spawnPoint.GameObject;
    }
}
