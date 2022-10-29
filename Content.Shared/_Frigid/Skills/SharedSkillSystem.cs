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
        foreach (var skillIdentifier in _publicSkills)
        {
            RetrieveSkillDataPrototype(skillIdentifier, out var prototype);
            if (prototype == null)
                continue;

            var skillData = new Skill(
                prototype.Name,
                prototype.DefaultLevel,
                prototype.MaxLevel,
                prototype.DefaultXP,
                prototype.MaxExperience,
                prototype.DisplayInSkills,
                prototype.ID
            );

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
        if (_skillData.TryGetValue(identifier, out var skillOrNot))
        {
            skill = skillOrNot;
            return true;
        }

        skill = null;
        return false;
    }

    public ushort GetLevel(SharedSkillsComponent comp, string id)
    {
        return comp.Skills.Find(i => i.ID == id).Level;
    }

    public void SetLevel(SharedSkillsComponent comp, string id, ushort level)
    {
        var skill = comp.Skills.Find(i => i.ID == id);
        skill.Level = level;

        Dirty(comp);
    }

    private protected void LoadPrototypes()
    {
        _skillData.Clear();
        _publicSkills.Clear();
        foreach(var skill in _prototypeManager.EnumeratePrototypes<SkillDataPrototype>())
        {
            if(_skillData.ContainsKey(skill.ID))
            {
                Logger.ErrorS("skills",
                    "Found skill with duplicate SkillDataPrototype ID {0} - all skills must have" +
                    " a unique prototype, this one will be skipped", skill.ID);
            }
            else
            {
                _sawmill.Log(LogLevel.Info, "Added skill prototype with {0} name", skill.ID);
                _skillData.Add(skill.ID, skill);
                if (skill.DisplayInSkills)
                    _publicSkills.Add(skill.ID);
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

        public string ID { get; }

        public ushort Level { get; set; }
        public ushort Experience { get; set; }

        public bool DisplayInMenu { get; }

        public ushort MaxLevel { get; }
        public ushort MaxExperience { get; }

        public Skill(string name, ushort level, ushort maxLevel, ushort exp, ushort maxExp, bool display, string id)
        {
            Name = name;
            Level = level;
            Experience = exp;
            MaxExperience = maxExp;
            MaxLevel = maxLevel;
            DisplayInMenu = display;
            ID = id;
        }
    }
}
