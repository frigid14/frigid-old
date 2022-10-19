using System.Linq;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.StationEvents.Events;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.StationEvents
{
    /// <summary>
    ///     The zombie event scheduler rule, copied from BasicStationEventSchedulerSystem
    /// </summary>
    [UsedImplicitly]
    public sealed class ZombieStationEventSchedulerSystem : GameRuleSystem
    {
        public override string Prototype => "ZombieStationEventScheduler";

        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly EventManagerSystem _event = default!;
        [Dependency] private readonly IPrototypeManager _prototype = default!;

        private const float MinimumTimeUntilFirstEvent = 300;

        /// <summary>
        /// How long until the next check for an event runs
        /// </summary>
        /// Default value is how long until first event is allowed
        [ViewVariables(VVAccess.ReadWrite)]
        private float _timeUntilNextEvent = MinimumTimeUntilFirstEvent;

        public override void Started() { }

        public override void Ended()
        {
            _timeUntilNextEvent = MinimumTimeUntilFirstEvent;
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            if (!RuleStarted || !_event.EventsEnabled)
                return;

            if (_timeUntilNextEvent > 0)
            {
                _timeUntilNextEvent -= frameTime;
                return;
            }

            _prototype.TryIndex<GameRulePrototype>("Incursion", out var proto);

            DebugTools.AssertNotNull(proto);
            if (proto != null)
                _event.GameTicker.AddGameRule(proto);
            ResetTimer();
        }

        /// <summary>
        /// Reset the event timer once the event is done.
        /// </summary>
        private void ResetTimer()
        {
            // 3 - 10 minutes.
            _timeUntilNextEvent = _random.Next(180, 600);
        }
    }
}
