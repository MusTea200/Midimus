using System.Collections.Generic;

namespace GameSystems.CharacterSystem
{
    public class MindVHSTape
    {
        public string OriginalName { get; private set; }
        public List<TraitType> Traits { get; private set; }
        public List<UpgradeableTrait> AdvancedTraits { get; private set; }
        public Dictionary<AttributeType, int> Attributes { get; private set; }

        public MindVHSTape(string originalName, List<TraitType> traits, List<UpgradeableTrait> advancedTraits, Dictionary<AttributeType, int> attributes)
        {
            OriginalName = originalName;
            Traits = new List<TraitType>(traits);
            AdvancedTraits = new List<UpgradeableTrait>(advancedTraits);
            Attributes = new Dictionary<AttributeType, int>(attributes);
        }
    }
}
