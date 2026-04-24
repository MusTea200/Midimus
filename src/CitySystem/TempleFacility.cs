using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class TempleFacility : CityFacility
    {
        private DailyLedger _ledger;

        public TempleFacility(DailyLedger ledger)
            : base("Tapınak", 2000)
        {
            _ledger = ledger;
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name}, arınmak için Tapınak'a giriş yaptı.");
        }

        public void RemoveCurse(Character character, Item item)
        {
            if (!item.IsCursed || !item.IsBound)
            {
                Console.WriteLine("Bu eşya lanetli veya mühürlü değil.");
                return;
            }

            int removalCost = 1500; // Yüksek miktar

            if (_ledger.MainBalance >= removalCost)
            {
                _ledger.DeductBalance(removalCost);
                item.IsBound = false;
                item.IsCursed = false;
                character.EquippedItems.Remove(item);

                Console.WriteLine($"{removalCost} altın bağışlandı. Kutsal ritüel tamamlandı.");
                Console.WriteLine($"{item.ItemName} üzerindeki lanet kırıldı ve eşya paramparça oldu.");

                // Stat geri yükleme mantığı (Basitçe)
                if (item.ItemName == "Kör Öfke Yüzüğü")
                {
                    if (character.Attributes.ContainsKey(AttributeType.Strength)) character.Attributes[AttributeType.Strength] -= 50;
                    Console.WriteLine($"{character.Name}'nin kör edici öfkesi dindi (Güç azaldı, İrade ve Zeka yavaşça geri gelecek).");
                }
            }
            else
            {
                Console.WriteLine($"Laneti kaldırmak için yeterli bağış yok (Gereken: {removalCost}).");
            }
        }
    }
}
