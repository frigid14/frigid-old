using Content.Shared.Movement.Systems;
using Robust.Shared.Random;

namespace Content.Shared.Zombies;

public abstract class SharedZombieSystem : EntitySystem
{
    [Dependency] private IRobustRandom _robustRandom = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ZombieComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshSpeed);
    }

    private void OnRefreshSpeed(EntityUid uid, ZombieComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        var mod = _robustRandom.NextFloat(component.ZombieMovementRandomSpeedMinimum, component.ZombieMovementRandomSpeedMaximum);
        args.ModifySpeed(mod, mod);
    }
}
