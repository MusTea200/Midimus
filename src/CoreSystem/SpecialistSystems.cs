using System;
using System.Linq;
using GameSystems.CharacterSystem;

namespace GameSystems.CoreSystem
{
    public static class SpecialistSystems
    {
        private static Random _rng = new Random();

        // 1. AppraisalSystem (Ekspertiz/Antikacı)
        public static void AppraisalSystem(Character specialist, Item item)
        {
            if (!item.IsUnidentified)
            {
                Console.WriteLine("Bu eşya zaten tanımlanmış.");
                return;
            }

            var antiquarianTrait = specialist.AdvancedTraits.FirstOrDefault(t => t.TraitName == "Antiquarian");
            if (antiquarianTrait == null)
            {
                Console.WriteLine($"{specialist.Name} bir Antikacı değil.");
                return;
            }

            double revealPercentage = 0;

            switch (antiquarianTrait.CurrentTier)
            {
                case TraitTier.White:
                    revealPercentage = _rng.NextDouble() * 0.50; // 0 - 50%
                    break;
                case TraitTier.Yellow:
                    revealPercentage = 0.20 + (_rng.NextDouble() * 0.60); // 20 - 80%
                    break;
                case TraitTier.Turquoise:
                    revealPercentage = 0.50 + (_rng.NextDouble() * 0.50); // 50 - 100%
                    break;
                case TraitTier.Purple:
                    revealPercentage = 0.80 + (_rng.NextDouble() * 0.20); // 80 - 100%

                    if (_rng.NextDouble() <= 0.50)
                    {
                        item.RevealedStatValue += 2;
                        Console.WriteLine("Efsanevi Ekspertiz! Eşyaya +2 bonus stat eklendi.");
                    }
                    break;
            }

            item.RevealedStatValue += (int)(item.BaseStatValue * revealPercentage);
            item.IsUnidentified = false;

            Console.WriteLine($"{specialist.Name} eşyayı inceledi. Gerçek Güç: {item.RevealedStatValue} / {item.BaseStatValue}");

            antiquarianTrait.UseTrait();
        }

        // 2. RepairSystem (Tamirci)
        public static void RepairSystem(Character specialist, Item item, DailyLedger ledger)
        {
            if (!item.IsBroken)
            {
                Console.WriteLine("Bu eşya zaten sağlam.");
                return;
            }

            var repairmanTrait = specialist.AdvancedTraits.FirstOrDefault(t => t.TraitName == "Repairman");
            if (repairmanTrait == null)
            {
                Console.WriteLine($"{specialist.Name} bir Tamirci değil.");
                return;
            }

            // Tier'a göre indirim (Örn: Base cost 100)
            int repairCost = 100;
            switch (repairmanTrait.CurrentTier)
            {
                case TraitTier.White: repairCost = 100; break;
                case TraitTier.Yellow: repairCost = 80; break;
                case TraitTier.Turquoise: repairCost = 50; break;
                case TraitTier.Purple: repairCost = 0; break; // Bedava
            }

            if (ledger.MainBalance >= repairCost)
            {
                ledger.DeductBalance(repairCost);
                item.IsBroken = false;
                Console.WriteLine($"{specialist.Name} eşyayı {repairCost} altın karşılığında tamir etti! Eşya artık %100 verimli.");
                repairmanTrait.UseTrait();
            }
            else
            {
                Console.WriteLine("Tamir için yeterli altın yok.");
            }
        }

        // 3. EnchantmentSystem (Efsuncu)
        public static void EnchantmentSystem(Character specialist, Item item, DailyLedger ledger)
        {
            var enchanterTrait = specialist.AdvancedTraits.FirstOrDefault(t => t.TraitName == "Enchanter");
            if (enchanterTrait == null)
            {
                Console.WriteLine($"{specialist.Name} bir Efsuncu değil.");
                return;
            }

            int enchantCost = 200;
            if (ledger.MainBalance >= enchantCost)
            {
                ledger.DeductBalance(enchantCost);

                int elementalBoost = _rng.Next(5, 21); // 5-20 arası rastgele hasar

                // Tier'a göre ek bonus eklenebilir
                if (enchanterTrait.CurrentTier == TraitTier.Purple) elementalBoost += 10;

                item.ElementalDamage += elementalBoost;
                Console.WriteLine($"{specialist.Name} eşyayı efsunladı! +{elementalBoost} Element Hasarı kazandı.");

                enchanterTrait.UseTrait();
            }
            else
            {
                Console.WriteLine("Efsunlama için yeterli altın yok.");
            }
        }
    }
}
