using System;
using System.Collections.Generic;
using GameSystems.CoreSystem;

namespace GameSystems.RealEstateSystem
{
    public class RealEstateManager
    {
        private List<DungeonProperty> _ownedDungeons;
        private List<DungeonProperty> _marketDungeons; // Satın alınabilecek zindanlar
        private Random _rng;

        public IReadOnlyList<DungeonProperty> OwnedDungeons => _ownedDungeons.AsReadOnly();

        public RealEstateManager()
        {
            _ownedDungeons = new List<DungeonProperty>();
            _marketDungeons = new List<DungeonProperty>();
            _rng = new Random();
        }

        public void AddDungeonToMarket(DungeonProperty dungeon)
        {
            _marketDungeons.Add(dungeon);
        }

        // Zindan satın alma işlemi
        public bool PurchaseDungeon(DungeonProperty dungeon, DailyLedger ledger)
        {
            if (ledger.MainBalance >= dungeon.MarketValue && !_ownedDungeons.Contains(dungeon))
            {
                ledger.DeductBalance(dungeon.MarketValue);
                dungeon.TransferOwnership(true);
                _ownedDungeons.Add(dungeon);
                _marketDungeons.Remove(dungeon);

                Console.WriteLine($"{dungeon.Name} zindanı {dungeon.MarketValue} Altın karşılığında satın alındı.");
                return true;
            }

            Console.WriteLine("Yetersiz bakiye veya zindan zaten size ait.");
            return false;
        }

        // Gün sonu emlak ve makroekonomi döngüsü
        public int DailyEconomicCycle()
        {
            ApplyMarketVolatility();

            int totalMaintenanceCost = 0;
            foreach (var dungeon in _ownedDungeons)
            {
                totalMaintenanceCost += dungeon.MaintenanceCost;
            }

            Console.WriteLine($"Günlük Emlak Giderleri: {totalMaintenanceCost} Altın kesildi.");
            return totalMaintenanceCost;
        }

        // Zindan fiyatlarında %5 ile %10 arası dalgalanma yaratır
        private void ApplyMarketVolatility()
        {
            foreach (var dungeon in _marketDungeons)
            {
                dungeon.ApplyMarketVolatility(CalculateStrictVolatility());
            }

            foreach (var dungeon in _ownedDungeons)
            {
                dungeon.ApplyMarketVolatility(CalculateStrictVolatility());
            }
        }

        private float CalculateStrictVolatility()
        {
            // 0.05 ile 0.10 arası rastgele bir oran
            float offset = 0.05f + (float)(_rng.NextDouble() * 0.05);
            // Yüzde 50 ihtimalle pozitif, yüzde 50 ihtimalle negatif dalgalanma
            if (_rng.NextDouble() > 0.5)
            {
                return 1.0f + offset;
            }
            return 1.0f - offset;
        }

        // Zindana giren kahramanlardan kazanılan komisyon/giriş ücreti (Örnek Gelir)
        public int CalculateDailyCommissions()
        {
            int totalCommissions = 0;
            foreach (var dungeon in _ownedDungeons)
            {
                // Zindanın prestijine göre günlük rastgele bir kazanç
                totalCommissions += dungeon.Prestige * _rng.Next(10, 50);
            }
            return totalCommissions;
        }
    }
}
