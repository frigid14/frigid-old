using Robust.Shared.Random;
using System.Linq;
using Content.Shared.Dataset;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.StationEvents.Events;

public sealed class RadioFluff : StationEventSystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override string Prototype => "RadioFluff";

    private string[] MakeCodewords()
    {
        var codewordCount = 4;
        var adjectives = _prototypeManager.Index<DatasetPrototype>("adjectives").Values;
        var verbs = _prototypeManager.Index<DatasetPrototype>("verbs").Values;
        var codewordPool = adjectives.Concat(verbs).ToList();
        var finalCodewordCount = Math.Min(codewordCount, codewordPool.Count);
        var codewords = new string[finalCodewordCount];
        for (var i = 0; i < finalCodewordCount; i++)
        {
            codewords[i] = RobustRandom.PickAndTake(codewordPool);
        }

        return codewords;
    }

    public override void Started()
    {
        base.Started();

        var message = Loc.GetString($"event-fluff-announcement-message-{RobustRandom.Next(1, 10)}");
        if (RobustRandom.Prob(0.15f))
        {
            var codewords = MakeCodewords();
            message = "";

            foreach (var codeword in codewords)
            {
                message += codeword.ToUpper() + " ";
            }

            ChatSystem.DispatchGlobalAnnouncement(message, sender: "Unknown Signal", playSound: false, colorOverride: Color.Red);
            SoundSystem.Play("/Audio/Misc/notice2.ogg", Filter.Broadcast());
        }
        else
        {
            ChatSystem.DispatchGlobalAnnouncement(message, sender: "Survival Radio", playSound: false, colorOverride: Color.PaleVioletRed);
            SoundSystem.Play("/Audio/Misc/notice2.ogg", Filter.Broadcast());
        }
    }
}
