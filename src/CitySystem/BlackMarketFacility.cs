using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class BlackMarketFacility : CityFacility
    {
        private DailyLedger _ledger;
        private Random _rng;

        public BlackMarketFacility(DailyLedger ledger)
            : base("Karaborsa", 1500)
        {
            _ledger = ledger;
            _rng = new Random();
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name}, tekinsiz Karaborsa'ya giriş yaptı.");
        }

        public Item? BuyMysteryItem(int cost)
        {
            if (_ledger.MainBalance >= cost)
            {
                _ledger.DeductBalance(cost);

                int randomBaseStat = _rng.Next(5, 21); // Rastgele stat
                Item mysteryItem;
                if (_rng.NextDouble() <= 0.30)
                {
                    int curseType = _rng.Next(0, 3);
                    if (curseType == 0) mysteryItem = new Item("Çivili Kefen", randomBaseStat);
                    else if (curseType == 1) mysteryItem = new Item("Kör Öfke Yüzüğü", randomBaseStat);
                    else mysteryItem = new Item("Midas'ın Gözyaşı", randomBaseStat);
                    mysteryItem.IsCursed = true;
                }
                else
                {
                    mysteryItem = new Item($"Gizemli Kutu Eşyası #{_rng.Next(100, 999)}", randomBaseStat);
                }

                // Karaborsadan alınan eşyalar %100 Unidentified ve rastgele Broken
                mysteryItem.IsUnidentified = true;
                mysteryItem.IsBroken = _rng.NextDouble() <= 0.40; // %40 ihtimalle kırık

                Console.WriteLine($"{cost} altın ödendi. Karaborsadan şüpheli bir eşya alındı!");
                return mysteryItem;
            }
            else
            {
                Console.WriteLine("Karaborsada veresiye geçmez. Yeterli altınınız yok.");
                return null;
            }
        }
    }
}
