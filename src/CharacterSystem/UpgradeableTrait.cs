using System;

namespace GameSystems.CharacterSystem
{
    public class UpgradeableTrait
    {
        public string TraitName { get; private set; }
        public TraitTier CurrentTier { get; private set; }
        public int UsageCount { get; private set; }

        private static Random _rng = new Random();

        public UpgradeableTrait(string traitName, TraitTier startingTier = TraitTier.White)
        {
            TraitName = traitName;
            CurrentTier = startingTier;
            UsageCount = 0;
        }

        public void UseTrait()
        {
            UsageCount++;
            TryLevelUp();
        }

        public void TryLevelUp()
        {
            if (UsageCount < 30) return;

            if (CurrentTier == TraitTier.Purple) return; // Zaten en yüksek seviye

            // 30 kullanım ve sonrası her kullanımda %20 şans
            if (_rng.NextDouble() <= 0.20)
            {
                CurrentTier++;
                UsageCount = 0;
                Console.WriteLine($"TEBRİKLER! '{TraitName}' yeteneği seviye atladı ve {CurrentTier} oldu!");
            }
        }
    }
}
