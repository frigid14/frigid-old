using System.Linq;
using Content.Server.Actions;
using Content.Server.Administration.Logs;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.RoundEnd;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Database;
using Content.Shared.GameTicking;
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
    [ViewVariables] private readonly List<EntityUid> _safezoneEntities = new();
    [ViewVariables] private readonly List<EntityUid> _zoneEntities = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SafezoneComponent, SafezoneTeleportActionEvent>(OnActionPerform);
        SubscribeLocalEvent<SafezoneComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SafezoneIdentifierComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<RoundEndSystemChangedEvent>(OnRoundEnd);
    }

    private void OnMapInit(EntityUid uid, SafezoneIdentifierComponent component, MapInitEvent args)
    {
        if (component.Safezone)
        {
            _safezoneEntities.Add(uid);
        }
        else
        {
            _zoneEntities.Add(uid);
        }
    }

    private void OnRoundEnd(RoundEndSystemChangedEvent args)
    {
        _safezoneEntities.Clear();
        _zoneEntities.Clear();
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

    /// <summary>
    /// TeleportToSafezone but with a doafter
    /// </summary>
    /// <param name="user"></param>
    /// <param name="comp"></param>
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

    /// <summary>
    /// Teleports an entity to the Safezone, and flips the component
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="comp"></param>
    public void TeleportToSafezone(EntityUid entity, SafezoneComponent comp)
    {
        comp.InSafezone = !comp.InSafezone;

        // Teleport
        var entityTransform = Transform(entity);
        var zoneEntity = EntityUid.Invalid;

        if (comp.InSafezone)
        {
            zoneEntity = _robustRandom.Pick(_safezoneEntities);
        }
        else
        {
            zoneEntity = _robustRandom.Pick(_zoneEntities);
        }

        var transform = Transform(zoneEntity);
        entityTransform.WorldPosition = transform.WorldPosition;

        entityTransform.AttachToGridOrMap();
    }
}

