using System;
using GameSystems.CharacterSystem;

namespace GameSystems.QuestSystem
{
    public abstract class InsurancePolicy
    {
        public Character InsuredCharacter { get; protected set; }
        public Quest TargetQuest { get; protected set; }

        public int Premium { get; protected set; } // Başarı durumunda oyuncunun kazanacağı prim
        public int Compensation { get; protected set; } // Ölüm durumunda oyuncunun ödeyeceği canlandırma tazminatı

        // Teklif Kaydırıcısı (1 - 100). 50 dengeli, >50 oyuncuya kârlı, <50 karaktere kârlı
        public int SliderValue { get; private set; }

        public InsurancePolicy(Character character, Quest quest)
        {
            InsuredCharacter = character;
            TargetQuest = quest;
            SliderValue = 50; // Varsayılan değer
            CalculatePolicy();
        }

        public void UpdateSlider(int newValue)
        {
            SliderValue = Math.Clamp(newValue, 1, 100);
            CalculatePolicy();
        }

        protected virtual void CalculatePolicy()
        {
            float riskFactor = CalculateRisk();

            // Temel Prim ve Tazminat hesaplaması
            int basePremium = (int)(TargetQuest.BaseReward * 0.2f * (1 + riskFactor));
            int baseCompensation = (int)(TargetQuest.BaseReward * 1.5f * (1 + riskFactor));

            // Slider 50 referans alınarak oran hesaplanır.
            // Slider artarsa (örn. 80), prim artar (+%60), tazminat azalır (-%60).
            float sliderMultiplier = (SliderValue - 50) / 50f; // -0.98 ile 1.0 arası

            Premium = (int)(basePremium * (1 + sliderMultiplier));
            Compensation = (int)(baseCompensation * (1 - sliderMultiplier));
        }

        protected bool _effectsApplied = false;

        // Yan etkiler Submit Offer sırasında çağrılır
        public virtual void ApplyPolicyEffects()
        {
            if (_effectsApplied) return;

            // Eğer teklif oyuncu lehine çok riskli/kârlı ise (Örn Slider > 80), karakter strese girer
            if (SliderValue > 80)
            {
                // Kırmızı bölge: Orantılı olarak stres ekle
                int stressPenalty = (SliderValue - 80) / 2;
                InsuredCharacter.Stress += stressPenalty;
            }

            _effectsApplied = true;
        }

        // Matematiksel Risk Fonksiyonu
        private float CalculateRisk()
        {
            float totalDeficit = 0f;
            int reqCount = 0;

            // Karakterin yeteneklerinin görev gereksinimlerini ne kadar karşıladığını hesapla
            foreach (var req in TargetQuest.Requirements)
            {
                reqCount++;
                int charStat = InsuredCharacter.Attributes[req.Key];
                if (charStat < req.Value)
                {
                    totalDeficit += (req.Value - charStat) / (float)req.Value; // Eksiklik oranı
                }
            }

            float baseRisk = TargetQuest.BaseDanger / 100f;
            float statRisk = reqCount > 0 ? (totalDeficit / reqCount) : 0;

            // Stres/Uyum seviyesi riski doğrudan etkiler (Stresli kahraman hata yapmaya meyillidir)
            float complianceMultiplier = 1.2f - InsuredCharacter.Compliance; // 0.2 ile 1.2 arası çarpan

            return Math.Clamp((baseRisk + statRisk) * complianceMultiplier, 0.1f, 1.0f);
        }
    }
}
