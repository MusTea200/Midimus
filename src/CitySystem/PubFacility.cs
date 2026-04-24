using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class PubFacility : RehabFacility
    {
        private Random _rng;

        public PubFacility(DailyLedger ledger)
            : base("Pub", 800, ledger, 150) // Medium cost
        {
            _rng = new Random();
        }

        public void Socialize(Character charA, Character charB, RelationshipManager relationshipManager)
        {
            if (charA.EquippedItems.Any(i => i.ItemName == "Kör Öfke Yüzüğü") || charB.EquippedItems.Any(i => i.ItemName == "Kör Öfke Yüzüğü"))
            {
                Console.WriteLine("Bu karakter kimseyle bağ kuramayacak kadar öfkeli!");
                return;
            }

            if (Ledger.MainBalance < GoldCost)
            {
                Console.WriteLine($"Sosyalleşmek için yeterli altın yok. Gereken: {GoldCost}, Mevcut: {Ledger.MainBalance}");
                return;
            }

            Ledger.DeductBalance(GoldCost);

            ApplyRehabEffects(charA);
            ApplyRehabEffects(charB);

            RelationshipBond bond = relationshipManager.GetOrCreateBond(charA, charB);
            bond.BondLevel += 1;

            Console.WriteLine($"{charA.Name} ve {charB.Name} Pubda sosyalleşti! Bağ seviyesi {bond.BondLevel} oldu.");
        }

        protected override void ApplyRehabEffects(Character character)
        {
            double roll = _rng.NextDouble();

            if (roll <= 0.15) // 15% chance of Bar Brawl
            {
                character.Stress += 20;
                int brawlPenalty = 100;

                if (Ledger.MainBalance >= brawlPenalty)
                {
                    Ledger.DeductBalance(brawlPenalty);
                }
                else
                {
                    Ledger.DeductBalance(Ledger.MainBalance); // Deduct all if not enough
                }

                Console.WriteLine($"EYVAH! {character.Name} Pub'da kavgaya karıştı! Stres arttı (+20) ve bar hasarı için kasadan ekstra altın ({brawlPenalty}) kesildi.");
            }
            else
            {
                character.Stress -= 40;
                Console.WriteLine($"{character.Name} Pub'da rahatladı. Stres azaldı (-40).");
            }
        }
    }
}
