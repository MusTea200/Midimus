using System;

namespace GameSystems.CharacterSystem
{
    public class Mount
    {
        public string Name { get; private set; }
        public int Price { get; private set; }

        // 0.0 ile 1.0 arası bir çarpan (Örn: %20 azalma için 0.20f)
        public float StressReductionPercentage { get; private set; }

        public Mount(string name, int price, float stressReductionPercentage)
        {
            Name = name;
            Price = price;
            StressReductionPercentage = Math.Clamp(stressReductionPercentage, 0f, 1.0f);
        }
    }
}
