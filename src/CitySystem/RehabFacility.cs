using System;
using System.Linq;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public abstract class RehabFacility : CityFacility
    {
        protected DailyLedger Ledger { get; private set; }
        public int GoldCost { get; protected set; }

        protected RehabFacility(string name, int baseUpgradeCost, DailyLedger ledger, int goldCost)
            : base(name, baseUpgradeCost)
        {
            Ledger = ledger;
            GoldCost = goldCost;
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name}, {FacilityName} tesisine giriş yaptı.");
        }

        public virtual bool RelaxCharacter(Character character, GameSystems.CoreSystem.PoliceManager? policeManager = null)
        {
            if (character.EquippedItems.Any(i => i.ItemName == "Midas'ın Gözyaşı"))
            {
                Console.WriteLine($"{character.Name} Midas'ın Gözyaşı'nın etkisi altında! Eğlence tesislerine giremez.");
                return false;
            }

            if (policeManager != null)
            {
                bool hasAcclimation = System.Linq.Enumerable.Any(System.Linq.Enumerable.OfType<GameSystems.CharacterSystem.BodyAcclimatizationTrait>(character.AdvancedTraits));
                if (hasAcclimation && (new System.Random()).NextDouble() <= 0.20) policeManager.TriggerPoliceInterrogation(character, Ledger);
            }

            if (Ledger.MainBalance < GoldCost)
            {
                Console.WriteLine($"Rehabilitasyon için yeterli altın yok. Gereken: {GoldCost}, Mevcut: {Ledger.MainBalance}");
                return false;
            }

            Ledger.DeductBalance(GoldCost);
            ApplyRehabEffects(character);
            return true;
        }

        protected abstract void ApplyRehabEffects(Character character);
    }
}
