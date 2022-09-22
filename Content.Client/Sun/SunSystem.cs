using Content.Shared.Eye.Blinding;
using Robust.Client.Graphics;

namespace Content.Client.Sun;

public sealed class SunSystem : EntitySystem
{
    [Dependency] ILightManager _lightManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BlindableComponent, ComponentInit>(OnSunInit);
    }

    private void OnSunInit(EntityUid uid, BlindableComponent component, ComponentInit args)
    {
        // The sun is bright. I don't like it. It's cool and all but I don't like how we have a massive ball of pure agonizing fire in the sky.
        _lightManager.AmbientLightColor = Color.Gray;
    }
}
