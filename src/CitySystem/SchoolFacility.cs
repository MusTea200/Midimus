using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class SchoolFacility : CityFacility
    {
        public SchoolFacility()
            : base("Okul", 2000)
        {
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name} okula giriş yaptı.");
        }

        public void SearchForSpecialist(DailyLedger ledger, JobCenterFacility jobCenter, string specialistType)
        {
            int searchCost = 250;
            if (ledger.MainBalance >= searchCost)
            {
                ledger.DeductBalance(searchCost);

                Character specialist = new Character($"{specialistType} Öğretmeni {Guid.NewGuid().ToString().Substring(0,4)}");
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
