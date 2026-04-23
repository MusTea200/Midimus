using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class MountDealershipFacility : CityFacility
    {
        private DailyLedger _ledger;
        public List<Mount> AvailableMounts { get; private set; }

        public MountDealershipFacility(DailyLedger ledger)
            : base("Binek Bayisi", 800)
        {
            _ledger = ledger;
            AvailableMounts = new List<Mount>
            {
                new Mount("Sadık At", 100, 0.20f),
                new Mount("Savaş Kurdu", 300, 0.35f),
                new Mount("Genç Ejderha", 1000, 0.50f)
            };
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name}, binek bakmak için Binek Bayisi'ne girdi.");
        }

        public bool PurchaseMount(Character character, Mount mount)
        {
            if (!AvailableMounts.Contains(mount))
            {
                Console.WriteLine("Bu binek şu an stokta yok.");
                return false;
            }

            if (_ledger.MainBalance < mount.Price)
            {
                Console.WriteLine("Bu bineği almak için yeterli Altınınız yok.");
                return false;
            }

            // Karakter aşırı stresliyse (%80 üzeri) bineği reddetme ihtimali (%30)
            if (character.Stress > 80)
            {
                Random rng = new Random();
                if (rng.NextDouble() <= 0.30)
                {
                    Console.WriteLine($"{character.Name} çok stresli olduğu için hayvanlarla uğraşmak istemediğini söyleyip bineği reddetti!");
                    return false;
                }
            }

            // Satın alım başarılı
            _ledger.DeductBalance(mount.Price);
            character.ActiveMount = mount;

            Console.WriteLine($"{character.Name}, {mount.Price} Altın karşılığında '{mount.Name}' bineğini kuşandı. (Stres Azaltma: %{mount.StressReductionPercentage * 100})");
            return true;
        }
    }
}
