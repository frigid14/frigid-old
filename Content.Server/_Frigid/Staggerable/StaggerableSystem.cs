using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared.Damage;
using Content.Shared.Popups;
using Robust.Shared.Player;

namespace Content.Server._Frigid.Staggerable;

public sealed class StaggerableSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly StunSystem _stunSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StaggerableComponent, DamageChangedEvent>(OnDamageChange);
    }

    private void OnDamageChange(EntityUid uid, StaggerableComponent component, DamageChangedEvent args)
    {
        if (args.DamageDelta != null)
        {
            // var totalDamage = args.Damageable.TotalDamage;
            var damage = args.DamageDelta.Total;

            if (damage > 15 && !component.Staggered)
            {
                var message = Loc.GetString("staggered-single-message");

                _popupSystem.PopupEntity(Loc.GetString("staggered-single-message"), uid, PopupType.LargeCaution);
                _stunSystem.TryKnockdown(uid, TimeSpan.FromMilliseconds(damage.Int() * 100), true);
            }
        }
    }
}
