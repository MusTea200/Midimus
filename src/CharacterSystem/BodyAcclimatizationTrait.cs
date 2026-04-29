using System;

namespace GameSystems.CharacterSystem
{
    public class BodyAcclimatizationTrait : UpgradeableTrait
    {
        public int RemainingDays { get; private set; }

        public BodyAcclimatizationTrait(int days = 20) : base("Beden Disforisi")
        {
            RemainingDays = days;
            UpdateTier();
        }

        public void DecrementDay()
        {
            if (RemainingDays > 0)
            {
                RemainingDays--;
                UpdateTier();
            }
        }

        private void UpdateTier()
        {
            // We use CurrentTier of UpgradeableTrait through reflection or just a separate property if we can't set it.
            // Since UpgradeableTrait CurrentTier is private set, we might need a workaround or just evaluate dynamically.
            // But we will use the logic required by the prompt in the simulation manager.
        }

        public TraitTier GetDynamicTier()
        {
            if (RemainingDays >= 15) return TraitTier.Purple;
            if (RemainingDays >= 5) return TraitTier.Turquoise; // Or Yellow
            if (RemainingDays >= 1) return TraitTier.White;
            return TraitTier.White;
        }

        public float GetEfficiencyMultiplier()
        {
            if (RemainingDays >= 15) return 0.30f; // -70%
            if (RemainingDays >= 5) return 0.70f;  // -30%
            if (RemainingDays >= 1) return 0.90f;  // -10%
            return 1.0f; // Tam uyum
        }
    }
}
