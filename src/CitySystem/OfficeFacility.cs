using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class OfficeFacility : CityFacility
    {
        private SimulationManager _simManager;
        private DailyLedger _ledger;

        public int OfficeLevel { get; private set; }

        public OfficeFacility(SimulationManager simManager, DailyLedger ledger)
            : base("Merkez Ofis", 5000)
        {
            _simManager = simManager;
            _ledger = ledger;
            OfficeLevel = 1;
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name} yasal merkez ofisine giriş yaptı.");
        }

        public void UpgradeOfficeLegally(int goldCost)
        {
            if (_ledger.MainBalance >= goldCost)
            {
                _ledger.DeductBalance(goldCost);
                OfficeLevel++;

                // Kalıcı stat artışları
                _simManager.GlobalReputation += 10;

                Console.WriteLine($"[YASAL İNŞAAT BAŞARILI] Ofis genişletildi! Yeni Seviye: {OfficeLevel}.");
                Console.WriteLine("Daha zengin ve prestijli müşteriler ofise çekilmeye başlayacak.");
            }
            else
            {
                Console.WriteLine("Yetersiz bakiye. Yasal genişletme için daha fazla altın gerekiyor.");
            }
        }
    }
}
