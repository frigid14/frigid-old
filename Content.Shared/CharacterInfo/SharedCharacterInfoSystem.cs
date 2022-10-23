using Content.Shared._Frigid.Skills;
using Content.Shared.Objectives;
using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo;

[Serializable, NetSerializable]
public sealed class RequestCharacterInfoEvent : EntityEventArgs
{
    public readonly EntityUid EntityUid;

    public RequestCharacterInfoEvent(EntityUid entityUid)
    {
        EntityUid = entityUid;
    }
}

[Serializable, NetSerializable]
public sealed class CharacterInfoEvent : EntityEventArgs
{
    public readonly EntityUid EntityUid;
    public readonly string JobTitle;
    public readonly Dictionary<string, List<ConditionInfo>> Objectives;
    public readonly string Briefing;
    public readonly List<SharedSkillSystem.SkillStruct> Skills;

    public CharacterInfoEvent(EntityUid entityUid, string jobTitle, Dictionary<string, List<ConditionInfo>> objectives, string briefing, List<SharedSkillSystem.SkillStruct> skills)
    {
        EntityUid = entityUid;
        JobTitle = jobTitle;
        Objectives = objectives;
        Briefing = briefing;
        Skills = skills;
    }
}
