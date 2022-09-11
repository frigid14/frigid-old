using Content.Shared.MobState;
using Content.Shared.Zombies;
using Robust.Client.Graphics;

namespace Content.Client.Zombies;

public sealed class ZombieSystem : SharedZombieSystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ZombieComponent, MobStateChangedEvent>(OnMobState);
    }

    public void OnMobState(EntityUid uid, ZombieComponent component, MobStateChangedEvent args)
    {
        var mgr = IoCManager.Resolve<ILightManager>();
        if (!mgr.LockConsoleAccess)
        {
            if (args.CurrentMobState == DamageState.Alive)
            {
                mgr.Enabled = false;
            }
            else
            {
                mgr.Enabled = true;
            }
        }
    }
}
