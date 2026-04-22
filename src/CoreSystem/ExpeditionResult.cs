using System;
using GameSystems.QuestSystem;

namespace GameSystems.CoreSystem
{
    public enum AnimationTriggerType
    {
        InstantWin,
        InstantLoss,
        Borderline,
        NormalBounce,
        MiracleBounce
    }

    public class ExpeditionResult
    {
        public InsurancePolicy Policy { get; private set; }
        public bool IsSuccess { get; private set; }
        public AnimationTriggerType AnimationState { get; private set; }

        // Ledger'da gösterilecek tutar (Başarıysa Prim +, Başarısızlıksa Tazminat -)
        public int LedgerAmount => IsSuccess ? Policy.Premium : -Policy.Compensation;

        public ExpeditionResult(InsurancePolicy policy, bool isSuccess, AnimationTriggerType animState)
        {
            Policy = policy;
            IsSuccess = isSuccess;
            AnimationState = animState;
        }
    }
}
