using System;
using GameSystems.CharacterSystem;

namespace GameSystems.QuestSystem
{
    public class InsurancePolicy
    {
        public Character InsuredCharacter { get; private set; }
        public Quest TargetQuest { get; private set; }

        public int Premium { get; private set; } // Başarı durumunda oyuncunun kazanacağı prim
        public int Compensation { get; private set; } // Ölüm durumunda oyuncunun ödeyeceği canlandırma tazminatı

        public InsurancePolicy(Character character, Quest quest)
        {
            InsuredCharacter = character;
            TargetQuest = quest;
            CalculatePolicy();
        }

        private void CalculatePolicy()
        {
            float riskFactor = CalculateRisk();

            // Risk ne kadar yüksekse, prim ve tazminat da o oranda artar
            Premium = (int)(TargetQuest.BaseReward * 0.2f * (1 + riskFactor));
            Compensation = (int)(TargetQuest.BaseReward * 1.5f * (1 + riskFactor));
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
