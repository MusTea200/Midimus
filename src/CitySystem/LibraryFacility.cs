using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class LibraryFacility : TrainingFacility
    {
        private static readonly List<AttributeType> _trainableStats = new List<AttributeType>
        {
            AttributeType.Intelligence,
            AttributeType.Willpower
        };

        protected override List<AttributeType> TrainableStats => _trainableStats;

        public LibraryFacility(DailyLedger ledger)
            : base("Kütüphane", 1200, ledger, 100, 15)
        {
        }

        // Future extension for Creature Training
        public virtual bool TrainCreature(object creature, AttributeType statToTrain)
        {
            // Placeholder for future Creature training logic
            Console.WriteLine("Yaratık eğitimi kütüphanede yakında aktif olacak.");
            return false;
        }

        public void SearchForSpecialist(DailyLedger ledger, JobCenterFacility jobCenter, string specialistType)
        {
            int searchCost = 250;
            if (ledger.MainBalance >= searchCost)
            {
                ledger.DeductBalance(searchCost);

                Character specialist = new Character($"{specialistType} Uzmanı {Guid.NewGuid().ToString().Substring(0,4)}");
                specialist.AdvancedTraits.Add(new UpgradeableTrait(specialistType));

                jobCenter.HeroPool.Add(specialist);
                Console.WriteLine($"{FacilityName}'de yapılan arama sonucu yeni bir {specialistType} bulundu ve İş Merkezi havuzuna eklendi!");
            }
            else
            {
                Console.WriteLine($"Uzman aramak için yeterli altın yok (Gereken: {searchCost}).");
            }
        }
    }
}
