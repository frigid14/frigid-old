using Content.Shared.Sun;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Player;

namespace Content.Server.Sun;

public sealed class SunSystem : SharedSunSystem
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedMapSystem _sharedMapSystem = default!;

    private float _accumulatedFrameTime;
    private float TimeUntilNextCycle = 300;
    private Time CurrentCycle = Time.Noon;

    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Cycles the current day/night cycle
    /// </summary>
    public void Cycle()
    {
        switch (CurrentCycle)
        {
            case Time.Day:
                CurrentCycle = Time.Noon;
                break;
            case Time.Noon:
                CurrentCycle = Time.Night;
                break;
            case Time.Night:
                CurrentCycle = Time.Midnight;
                break;
            case Time.Midnight:
                CurrentCycle = Time.Day;
                break;
        }

        UpdateLighting();

        Logger.Debug($"Cycled day night to {CurrentCycle.ToString()}");
    }

    /// <summary>
    /// Updates the lighting,
    /// </summary>
    public void UpdateLighting()
    {
        var color = MidnightColor;
        switch (CurrentCycle)
        {
            case Time.Midnight:
                color = MidnightColor;
                break;
            case Time.Day:
                color = DayColor;
                break;
            case Time.Noon:
                color = NoonColor;
                break;
            case Time.Night:
                color = NightColor;
                break;
        }
        // _sharedMapSystem.SetAmbientLight(mapId, color);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _accumulatedFrameTime += frameTime;

        if (_accumulatedFrameTime > TimeUntilNextCycle)
        {
            _accumulatedFrameTime -= TimeUntilNextCycle;
            Cycle();
        }
    }
}
