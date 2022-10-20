using Content.Server.StationEvents.Components;
using Robust.Shared.Random;
using System.Linq;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.Spawners.Components;
using Robust.Shared.Audio;
using Robust.Shared.Player;

namespace Content.Server.StationEvents.Events;

public sealed class Incursion : StationEventSystem
{
    public override string Prototype => "Incursion";

    public override void Started()
    {
        base.Started();

        // Spawn the zombers
        var spawnChoice = "MobZed";
        var spawnLocations = EntityManager.EntityQuery<TimedSpawnerComponent, TransformComponent>().ToList();
        RobustRandom.Shuffle(spawnLocations);

        var mod = Math.Sqrt(RobustRandom.Next(1,5));
        var spawnerCount = 0;

        foreach (var (spawner, transform) in spawnLocations)
        {
            if (!spawner.Prototypes.Contains("MobZed"))
                continue;

            var spawnAmount = (int) (RobustRandom.Next(1, 3) * mod);
            Sawmill.Info($"Spawning {spawnAmount} of {spawnChoice}");

            spawnerCount++;

            for (var i = 0; i < spawnAmount; i++)
            {
                if (spawnAmount <= 0)
                {
                    break;
                }

                spawnAmount--;
                EntityManager.SpawnEntity(spawnChoice, transform.Coordinates);

            }
        }

        // Alert the players
        ChatSystem.DispatchGlobalAnnouncement(Loc.GetString("event-incursion-announcement-message", ("influx", Math.Round(mod)), ("estimate",
            Math.Round((mod * 3) * spawnerCount))), sender: "Survival Radio", playSound: false, colorOverride: Color.PaleVioletRed);
        SoundSystem.Play("/Audio/Misc/siren.ogg", Filter.Broadcast());
    }
}
