using System;

namespace GameSystems.RealEstateSystem
{
    public class DungeonProperty
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public int BaseValue { get; private set; }
        public int Prestige { get; private set; } // Genellikle 10 üzerinden başlar
        public int MaintenanceCost { get; private set; }

        public bool IsOwnedByPlayer { get; private set; }

        // Piyasa değeri: BaseValue * (Prestige / 10). MarketVolatility çarpanıyla son haline getirilir.
        public int MarketValue => (int)(BaseValue * (Prestige / 10f) * _marketVolatilityMultiplier);

        private float _marketVolatilityMultiplier = 1.0f;

        public DungeonProperty(string name, int baseValue, int maintenanceCost)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            BaseValue = baseValue;
            MaintenanceCost = maintenanceCost;
            Prestige = 10;
            IsOwnedByPlayer = false;
        }

        public void Upgrade()
        {
            // Zindan yükseltildiğinde prestiji ve bakım maliyeti artar
            Prestige += 2;
            MaintenanceCost = (int)(MaintenanceCost * 1.2f);
            Console.WriteLine($"{Name} zindanı yükseltildi! Yeni Prestij: {Prestige}, Yeni Bakım Maliyeti: {MaintenanceCost}");
        }

        public void TransferOwnership(bool isOwned)
        {
            IsOwnedByPlayer = isOwned;
        }

        public void ApplyMarketVolatility(float multiplier)
        {
            _marketVolatilityMultiplier = multiplier;
        }
    }
}
