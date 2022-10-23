using Content.Shared.Alert;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Player;

namespace Content.Server.MobState;

public sealed partial class MobStateSystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    public override void EnterDeadState(EntityUid uid)
    {
        base.EnterDeadState(uid);

        Alerts.ShowAlert(uid, AlertType.HumanDead);

        EntityManager.TryGetComponent<TransformComponent>(uid, out var transformComponent);

        if (transformComponent != null)
        {
            _popup.PopupEntity(Loc.GetString("chat-manager-entity-deathgasp-wrap-message",
                ("entityName", uid)), uid, Filter.Empty().AddPlayersByPvs(transformComponent.Coordinates));
        }

        if (HasComp<StatusEffectsComponent>(uid))
        {
            Status.TryRemoveStatusEffect(uid, "Stun");
        }
    }
}
