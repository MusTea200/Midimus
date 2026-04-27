using System;
using System.Collections.Generic;

namespace GameSystems.CharacterSystem
{
    public class VatGrownBody
    {
        public string VatId { get; private set; }
        public bool IsHighRisk { get; private set; }
        public int RemainingDays { get; private set; }
        public bool IsReady { get; private set; }
        public bool IsDestroyed { get; private set; }
        public bool IsExpendableLabor { get; set; }

        // The body's base stats/buffs waiting to be harvested
        public Dictionary<AttributeType, int> BaseAttributes { get; private set; }
        public List<TraitType> BaseTraits { get; private set; }

        public VatGrownBody(bool isHighRisk)
        {
            VatId = "VAT-" + Guid.NewGuid().ToString().Substring(0, 4);
            IsHighRisk = isHighRisk;
            RemainingDays = isHighRisk ? 7 : 3;
            IsReady = false;
            IsDestroyed = false;
            IsExpendableLabor = false;

            BaseAttributes = new Dictionary<AttributeType, int>();
            BaseTraits = new List<TraitType>();

            // Initialize all core stats to a baseline of 10
            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
            {
                BaseAttributes[attr] = 10;
            }
        }

        public void DecrementDay()
        {
            if (IsReady || IsDestroyed) return;

            RemainingDays--;
            if (RemainingDays <= 0)
            {
                FinishGrowth();
            }
        }

        public void DestroyBody()
        {
            IsDestroyed = true;
            IsReady = false;
            Console.WriteLine($"[BİYOLOJİK HATA] {VatId} içindeki klon çürüdü/yok edildi.");
        }

        private void FinishGrowth()
        {
            IsReady = true;
            Random rng = new Random();
            Array attributes = Enum.GetValues(typeof(AttributeType));

            int buffCount = IsHighRisk ? rng.Next(0, 4) : rng.Next(0, 2); // 0-3 for high risk, 0-1 for low
            int debuffCount = IsHighRisk ? rng.Next(0, 4) : rng.Next(0, 2);

            for (int i = 0; i < buffCount; i++)
            {
                AttributeType buffStat = (AttributeType)attributes.GetValue(rng.Next(attributes.Length))!;
                int buffAmount = IsHighRisk ? rng.Next(15, 41) : rng.Next(5, 16);
                BaseAttributes[buffStat] += buffAmount;
            }

            for (int i = 0; i < debuffCount; i++)
            {
                AttributeType debuffStat = (AttributeType)attributes.GetValue(rng.Next(attributes.Length))!;
                int debuffAmount = IsHighRisk ? rng.Next(10, 31) : rng.Next(5, 11);
                BaseAttributes[debuffStat] -= debuffAmount;
            }

            Console.WriteLine($"[KLON TAMAMLANDI] {VatId} hasat edilmeye hazır. (Risk: {(IsHighRisk ? "Yüksek" : "Düşük")})");
        }
    }
}
