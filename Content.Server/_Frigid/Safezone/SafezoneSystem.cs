using Content.Server.Actions;
using Content.Server.Administration.Logs;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Database;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Frigid.Safezone;

public sealed class SafezoneSystem : EntitySystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly ActionsSystem _actionsSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    private EntityUid _safezoneTeleportEntity;
    private readonly List<EntityUid> _zoneEntities = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SafezoneComponent, SafezoneTeleportActionEvent>(OnActionPerform);
        SubscribeLocalEvent<SafezoneComponent, ComponentInit>(OnComponentInit);
        foreach (var identifier in EntityQuery<SafezoneIdentifierComponent>())
        {
            if (identifier.Safezone)
            {
                _safezoneTeleportEntity = identifier.Owner;
                continue;
            }
            _zoneEntities.Add(identifier.Owner);
        }
    }

    private void OnComponentInit(EntityUid uid, SafezoneComponent component, ComponentInit args)
    {
        _prototypeManager.TryIndex<InstantActionPrototype>(component.ActionId, out var action);

        if (action == null)
            return;

        _actionsSystem.AddAction(uid, new InstantAction(action), null);
    }

    private void OnActionPerform(EntityUid uid, SafezoneComponent component, SafezoneTeleportActionEvent args)
    {
        if (args.Handled)
            return;

        TryTeleportToSafezone(uid, component);

        args.Handled = true;
    }

    public async void TryTeleportToSafezone(EntityUid user, SafezoneComponent comp)
    {
        if (comp.EnteringSafezone)
            return;

        if (comp.InSafezone)
        {
            SoundSystem.Play(comp.EndTeleportSound.GetSound(), Filter.Pvs(user), user);
            _popupSystem.PopupEntity(Loc.GetString("safezone-teleport-success-message", ("targetName", user)), user, Filter.Pvs(user), PopupType.Medium);
            _adminLogger.Add(LogType.Action, LogImpact.Medium, $"{_entities.ToPrettyString(user):player} has teleported out of safezone");
            TeleportToSafezone(user, comp);
            return;
        }

        var cuffTime = 10f;

        var doAfterEventArgs = new DoAfterEventArgs(user, cuffTime, default)
        {
            BreakOnTargetMove = true,
            BreakOnUserMove = true,
            BreakOnDamage = true,
            BreakOnStun = true,
            NeedHand = true
        };

        SoundSystem.Play(comp.StartTeleportSound.GetSound(), Filter.Pvs(user), user);
        _popupSystem.PopupEntity(Loc.GetString("safezone-teleport-attempt-message", ("targetName", user)), user, Filter.Pvs(user), PopupType.Medium);

        comp.EnteringSafezone = true;
        var result = await _doAfter.WaitDoAfter(doAfterEventArgs);
        comp.EnteringSafezone = false;

        SoundSystem.Play(comp.EndTeleportSound.GetSound(), Filter.Pvs(user), user);

        if (result != DoAfterStatus.Cancelled)
        {
            _popupSystem.PopupEntity(Loc.GetString("safezone-teleport-success-message", ("targetName", user)), user, Filter.Pvs(user), PopupType.Medium);
            _adminLogger.Add(LogType.Action, LogImpact.Medium, $"{_entities.ToPrettyString(user):player} has teleported to safezone");
            TeleportToSafezone(user, comp);
        }
        else
        {
            _popupSystem.PopupEntity(Loc.GetString("safezone-teleport-failure-message", ("targetName", user)), user, Filter.Pvs(user), PopupType.Medium);
        }
    }

    public void TeleportToSafezone(EntityUid entity, SafezoneComponent comp)
    {
        // Teleport
        var entityTransform = Transform(entity);

        if (comp.InSafezone)
        {
            var safezoneTransform = Transform(_safezoneTeleportEntity);

            entityTransform.WorldPosition = safezoneTransform.WorldPosition;
        }
        else
        {
            var zoneEntity = _robustRandom.Pick(_zoneEntities);
            var zoneTransform = Transform(zoneEntity);

            entityTransform.WorldPosition = zoneTransform.WorldPosition;
        }
        entityTransform.AttachToGridOrMap();
        comp.InSafezone = !comp.InSafezone;
    }
}

