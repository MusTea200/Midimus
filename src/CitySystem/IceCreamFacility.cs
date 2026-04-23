using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class IceCreamFacility : RehabFacility
    {
        public IceCreamFacility(DailyLedger ledger)
            : base("Dondurmacı", 300, ledger, 30) // Low cost
        {
        }

        protected override void ApplyRehabEffects(Character character)
        {
            int stressRelief = 15;

            if (character.Traits.Contains(TraitType.Gluttonous) || character.Traits.Contains(TraitType.Childish))
            {
                stressRelief *= 2;
                Console.WriteLine($"{character.Name}'nin obur veya çocuksu doğası dondurmadan ekstra keyif almasını sağladı!");
            }

            character.Stress -= stressRelief;
            Console.WriteLine($"{character.Name} dondurma yedi. Stres azaldı (-{stressRelief}).");
        }
    }
}
