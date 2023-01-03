using Content.Shared.Chemistry.Reagent;
using Content.Server.Disease;
using JetBrains.Annotations;

namespace Content.Server.Chemistry.ReagentEffects;
/// <summary>
/// Default metabolism for medicine reagents.
/// </summary>
[UsedImplicitly]
public sealed class ChemTreatInfection : ReagentEffect
{
    public override void Effect(ReagentEffectArgs args)
    {
        var ev = new TreatInfectionAttemptEvent();
        args.EntityManager.EventBus.RaiseLocalEvent(args.SolutionEntity, ev, false);
    }
}
