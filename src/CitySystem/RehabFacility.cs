using System;
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

        public virtual bool RelaxCharacter(Character character)
        {
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
