using System.Collections.Generic;

namespace GameSystems.CharacterSystem
{
    public class MindDrive
    {
        public string OriginalName { get; private set; }
        public List<TraitType> Traits { get; private set; }
        public List<UpgradeableTrait> AdvancedTraits { get; private set; }

        public MindDrive(string originalName, List<TraitType> traits, List<UpgradeableTrait> advancedTraits)
        {
            OriginalName = originalName;
            Traits = new List<TraitType>(traits);
            AdvancedTraits = new List<UpgradeableTrait>(advancedTraits);
        }
    }
}
