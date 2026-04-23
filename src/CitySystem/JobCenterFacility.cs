using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class JobCenterFacility : CityFacility
    {
        private DailyLedger _ledger;
        public List<Character> HeroPool { get; private set; }

        public JobCenterFacility(DailyLedger ledger)
            : base("İş Merkezi", 500)
        {
            _ledger = ledger;
            HeroPool = new List<Character>();
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name} İş Merkezi'ne girdi (Belki kendi özelliklerini güncellemek istiyor).");
        }

        // Günlük karakter havuzunu para karşılığı yeniler
        public void RefreshHeroPool()
        {
            int refreshCost = 50;

            if (_ledger.MainBalance >= refreshCost)
            {
                _ledger.DeductBalance(refreshCost);
                HeroPool.Clear();

                // Rastgele 3 yeni kahraman oluştur
                for (int i = 0; i < 3; i++)
                {
                    HeroPool.Add(new Character($"Maceracı {Guid.NewGuid().ToString().Substring(0, 4)}"));
                }

                Console.WriteLine("İş Merkezi havuzu yenilendi! Yeni kahramanlar kapıda bekliyor.");
            }
            else
            {
                Console.WriteLine("Havuzu yenilemek için yeterli paranız yok (50 Altın gerekli).");
            }
        }

        // Özel bir özelliğe sahip karakteri premium fiyata işe aldırır
        public Character? RecruitSpecificTrait(TraitType desiredTrait)
        {
            int premiumCost = 300;

            if (_ledger.MainBalance >= premiumCost)
            {
                _ledger.DeductBalance(premiumCost);

                Character specialHero = new Character($"Özel Ajan {Guid.NewGuid().ToString().Substring(0, 4)}");

                // İstenen özelliği karaktere ekle
                if (!specialHero.Traits.Contains(desiredTrait))
                {
                    specialHero.Traits.Add(desiredTrait);
                }

                Console.WriteLine($"{desiredTrait} özelliğine sahip {specialHero.Name} {premiumCost} Altın karşılığında işe alındı!");
                return specialHero;
            }
            else
            {
                Console.WriteLine($"Özel karakter almak için yeterli paranız yok ({premiumCost} Altın gerekli).");
                return null;
            }
        }
    }
}
