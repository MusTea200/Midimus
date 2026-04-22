using System;
using System.Collections.Generic;
using GameSystems.QuestSystem;

namespace GameSystems.CoreSystem
{
    public class SimulationManager
    {
        private Random _rng;

        // UI için eventler (Sekizgen animasyon sistemine haber vermek için)
        public event Action<ExpeditionResult>? OnSimulationProcessed;
        public event Action<List<ExpeditionResult>>? OnDailySimulationCompleted;

        public SimulationManager()
        {
            _rng = new Random();
        }

        public List<ExpeditionResult> RunDailyExpeditions(List<InsurancePolicy> activePolicies)
        {
            List<ExpeditionResult> dailyResults = new List<ExpeditionResult>();

            foreach (var policy in activePolicies)
            {
                ExpeditionResult result = ProcessSingleExpedition(policy);
                dailyResults.Add(result);

                // UI'ın anlık sekme animasyonunu dinleyebilmesi için eventi tetikle
                OnSimulationProcessed?.Invoke(result);
            }

            // Günün tamamı bittiğinde bilanço için tetikle
            OnDailySimulationCompleted?.Invoke(dailyResults);

            return dailyResults;
        }

        private ExpeditionResult ProcessSingleExpedition(InsurancePolicy policy)
        {
            // Başarı ihtimalini belirlemek için kesişim oranı hesaplanmalı.
            // Bu simülasyon için basitleştirilmiş bir formül:
            // Kahramanın uyumu ve görev zorluğu üzerinden bir olasılık (0.0 - 1.0)

            float successProbability = CalculateSuccessProbability(policy);

            bool isSuccess;
            AnimationTriggerType animType;

            if (successProbability >= 1.0f)
            {
                isSuccess = true;
                animType = AnimationTriggerType.InstantWin;
            }
            else if (successProbability <= 0.0f)
            {
                isSuccess = false;
                animType = AnimationTriggerType.InstantLoss;
            }
            else
            {
                // Rastgele zar (RNG)
                double roll = _rng.NextDouble();
                isSuccess = roll <= successProbability;

                // Kıl payı durumu: Eğer zar sınıra %10 yakınsa Borderline gerilim animasyonu tetiklensin
                if (Math.Abs(roll - successProbability) <= 0.1)
                {
                    animType = AnimationTriggerType.Borderline;
                }
                else
                {
                    animType = AnimationTriggerType.NormalBounce;
                }
            }

            return new ExpeditionResult(policy, isSuccess, animType);
        }

        private float CalculateSuccessProbability(InsurancePolicy policy)
        {
            float totalDeficit = 0f;
            int reqCount = 0;

            foreach (var req in policy.TargetQuest.Requirements)
            {
                reqCount++;
                int charStat = policy.InsuredCharacter.Attributes[req.Key];
                if (charStat < req.Value)
                {
                    totalDeficit += (req.Value - charStat) / (float)req.Value;
                }
            }

            float baseDanger = policy.TargetQuest.BaseDanger / 100f;
            float statPenalty = reqCount > 0 ? (totalDeficit / reqCount) : 0;
            float complianceFactor = policy.InsuredCharacter.Compliance; // 0.0 - 1.0 arası

            // Başarı şansı = (1 - Tehlike - Stat Eksikliği) * Karakterin Uyumu
            float chance = (1f - baseDanger - statPenalty) * complianceFactor;

            return Math.Clamp(chance, 0f, 1f);
        }
    }
}
