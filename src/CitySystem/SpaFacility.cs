using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class SpaFacility : RehabFacility
    {
        public SpaFacility(DailyLedger ledger)
            : base("Lüks Spa", 2000, ledger, 500) // High cost
        {
        }

        protected override void ApplyRehabEffects(Character character)
        {
            character.Stress = 0;
            character.Patience = 100; // As requested, max patience. Character.cs setter will clamp this to 5.

            Console.WriteLine($"{character.Name} Lüks Spa'da tamamen yenilendi! Stres sıfırlandı ve sabrı maksimuma ulaştı.");
        }
    }
}
