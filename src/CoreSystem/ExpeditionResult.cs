using System;
using System.Collections.Generic;
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

        public Dictionary<CraftingMaterial, int> LootedMaterials { get; private set; }

        public ExpeditionResult(InsurancePolicy policy, bool isSuccess, AnimationTriggerType animState, Dictionary<CraftingMaterial, int>? lootedMaterials = null)
        {
            Policy = policy;
            IsSuccess = isSuccess;
            AnimationState = animState;
            LootedMaterials = lootedMaterials ?? new Dictionary<CraftingMaterial, int>();
        }
    }
}
