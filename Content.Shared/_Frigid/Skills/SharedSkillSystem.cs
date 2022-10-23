using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Frigid.Skills;

/// <summary>
/// The shared system for handling skills, levels and EXP.
/// </summary>
public abstract class SharedSkillSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private readonly Dictionary<string, SkillDataPrototype> _skillData = new();
    private readonly List<string> _publicSkills = new();
    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SharedSkillsComponent, ComponentInit>(OnComponentInit);

        _sawmill = Logger.GetSawmill("skills");
        _sawmill.Level = LogLevel.Info;

        LoadPrototypes();
        _prototypeManager.PrototypesReloaded += HandlePrototypesReloaded;
    }

    private void OnComponentInit(EntityUid uid, SharedSkillsComponent component, ComponentInit args)
    {
        base.Initialize();
        component.Skills.Clear();
        foreach(var skillIdentifier in _publicSkills)
        {
            RetrieveSkillDataPrototype(skillIdentifier, out var prototype);
            if (prototype == null)
                continue;

            var skillData = new Skill(skillIdentifier, prototype.DefaultLevel, prototype.MaxLevel, prototype.DefaultXP, prototype.MaxExperience, prototype.DisplayInSkills);
            component.Skills.Add(skillData);
        }
    }

    private protected void HandlePrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        LoadPrototypes();
    }

    /// <summary>
    /// Retrieves a SkillDataPrototype by it's identifier.
    /// </summary>
    /// <param name="identifier">string</param>
    /// <param name="skill">SkillDataPrototype</param>
    /// <returns>bool</returns>
    public bool RetrieveSkillDataPrototype(string identifier, [NotNullWhen(true)] out SkillDataPrototype? skill)
    {
        if(_skillData.TryGetValue(identifier, out var skillOrNot))
        {
            skill = skillOrNot;
            return true;
        }
        skill = null;
        return false;
    }

    private protected void LoadPrototypes()
    {
        _skillData.Clear();
        _publicSkills.Clear();
        foreach(var skill in _prototypeManager.EnumeratePrototypes<SkillDataPrototype>())
        {
            if(_skillData.ContainsKey(skill.Name))
            {
                Logger.ErrorS("skills",
                    "Found skill with duplicate SkillDataPrototype Name {0} - all skills must have" +
                    " a unique prototype, this one will be skipped", skill.Name);
            }
            else
            {
                _sawmill.Log(LogLevel.Info, "Added skill prototype with {0} name", skill.Name);
                _skillData.Add(skill.Name, skill);
                if (skill.DisplayInSkills)
                    _publicSkills.Add(skill.Name);
            }
        }
    }

    /// <summary>
    /// A struct for defining skills, use this instead of SkillsDataPrototype if you wish to store current levels/experience.
    /// </summary>
    [Serializable, NetSerializable]
    public struct Skill
    {
        public string Name { get; set; }
        public bool DisplayInMenu { get; set; }
        public ushort Level { get; set; }
        public ushort MaxLevel { get; set; }
        public ushort Experience { get; set; }
        public ushort MaxExperience { get; set; }

        public Skill(string name, ushort level, ushort maxLevel, ushort exp, ushort maxExp, bool display)
        {
            Name = name;
            Level = level;
            Experience = exp;
            MaxExperience = maxExp;
            MaxLevel = maxLevel;
            DisplayInMenu = display;
        }
    }
}
