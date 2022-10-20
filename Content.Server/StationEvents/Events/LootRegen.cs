using System.Linq;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Player;

namespace Content.Server.StationEvents.Events;

public sealed class LootRegen : StationEventSystem
{
    [Dependency] private readonly EntityManager _entityManager = default!;

    public override string Prototype => "LootRegen";

    public override void Started()
    {
        base.Started();
        var counters = EntityManager.EntityQuery<SharedStorageComponent, TransformComponent>().ToList();

        foreach (var (storage, transform) in counters)
        {
            var uid = storage.Owner;
            var worked = _entityManager.TryGetComponent<MetaDataComponent>(uid, out var meta);

            if (meta != null && meta.EntityPrototype?.ID == "CounterWoodHouseRandom")
            {
                _entityManager.SpawnEntity(meta.EntityPrototype?.ID, transform.Coordinates);
                _entityManager.DeleteEntity(uid);
            }
        }

        // Alert the players
        ChatSystem.DispatchGlobalAnnouncement(Loc.GetString("event-loot-regen-announcement-message"), sender: "Survival Radio", playSound: false, colorOverride: Color.PaleVioletRed);
        SoundSystem.Play("/Audio/Misc/notice2.ogg", Filter.Broadcast());
    }
}
