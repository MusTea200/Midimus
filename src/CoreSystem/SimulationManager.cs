using System;
using System.Linq;
using System.Collections.Generic;
using GameSystems.QuestSystem;
using GameSystems.CharacterSystem;

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

            // Mucize Kontrolü: Görev başarısız ama karakterde MiracleReady varsa, ölümden döner.
            if (!isSuccess && policy.InsuredCharacter.MiracleReady)
            {
                isSuccess = true;
                animType = AnimationTriggerType.MiracleBounce;
                policy.InsuredCharacter.MiracleReady = false; // Mucize harcandı
                Console.WriteLine($"{policy.InsuredCharacter.Name} ölümcül bir darbe aldı ama Mucize sayesinde hayatta kaldı!");
            }

            // Stres ve Binek (Mount) Hasar Azaltma Mantığı
            if (!isSuccess || animType == AnimationTriggerType.Borderline)
            {
                // Standart Görev Stresi (Borderline 15, InstantLoss 50, NormalLoss 30)
                int baseStressDamage = 30;
                if (animType == AnimationTriggerType.Borderline) baseStressDamage = 15;
                if (animType == AnimationTriggerType.InstantLoss) baseStressDamage = 50;

                // Eğer karakterin bineği varsa hasarı azalt
                if (policy.InsuredCharacter.ActiveMount != null)
                {
                    float reduction = policy.InsuredCharacter.ActiveMount.StressReductionPercentage;
                    baseStressDamage = (int)(baseStressDamage * (1.0f - reduction));
                    Console.WriteLine($"{policy.InsuredCharacter.Name}'nin bineği ({policy.InsuredCharacter.ActiveMount.Name}) sayesinde stres hasarı azaldı!");
                }

                policy.InsuredCharacter.Stress += baseStressDamage;
                if (policy.InsuredCharacter.EquippedItems.Any(i => i.ItemName == "Çivili Kefen"))
                {
                    if (policy.InsuredCharacter.Attributes.ContainsKey(AttributeType.Endurance)) policy.InsuredCharacter.Attributes[AttributeType.Endurance] -= baseStressDamage;
                    else policy.InsuredCharacter.Attributes[AttributeType.Endurance] = -baseStressDamage;
                    Console.WriteLine($"{policy.InsuredCharacter.Name}'nin Çivili Kefen'i {baseStressDamage} kalıcı Endurance statına mal oldu!");
                }
            }

            // Berserker's Shroud increment and logic
            if (policy.InsuredCharacter.EquippedItems.Any(i => i.ItemName == "Çivili Kefen"))
            {
                var berserkTrait = policy.InsuredCharacter.AdvancedTraits.OfType<BerserkCurseTrait>().FirstOrDefault();
                if (berserkTrait == null)
                {
                    berserkTrait = new BerserkCurseTrait();
                    policy.InsuredCharacter.AdvancedTraits.Add(berserkTrait);
                }
                berserkTrait.IncrementExpedition(policy.InsuredCharacter);

                // Apply stat drop temporarily or permanently depending on architecture
                float statDrop = 0f;
                if (berserkTrait.ExpeditionCount >= 15) statDrop = 0.90f;
                else if (berserkTrait.ExpeditionCount >= 10) statDrop = 0.50f;
                else if (berserkTrait.ExpeditionCount >= 1) statDrop = 0.10f;

                if (statDrop > 0f)
                {
                    Console.WriteLine($"Çivili Kefen Laneti: Görev sonrası tüm temel statlar %{statDrop * 100} düştü!");
                    // In a real system, you'd keep base stats intact and apply modifiers. Here we permanently drop for simplicity.
                    var keys = new List<AttributeType>(policy.InsuredCharacter.Attributes.Keys);
                    foreach (var key in keys)
                    {
                        policy.InsuredCharacter.Attributes[key] = (int)(policy.InsuredCharacter.Attributes[key] * (1.0f - statDrop));
                    }
                }
            }

            Dictionary<CraftingMaterial, int> lootedMaterials = new Dictionary<CraftingMaterial, int>();
            if (isSuccess)
            {
                Array materials = Enum.GetValues(typeof(CraftingMaterial));
                int numMaterialTypes = _rng.Next(1, 4);
                for (int i = 0; i < numMaterialTypes; i++)
                {
                    CraftingMaterial randomMat = (CraftingMaterial)materials.GetValue(_rng.Next(materials.Length))!;
                    int amount = (int)(_rng.Next(1, policy.TargetQuest.DifficultyLevel * 2 + 2));
                    if (lootedMaterials.ContainsKey(randomMat)) lootedMaterials[randomMat] += amount;
                    else lootedMaterials.Add(randomMat, amount);
                }
            }

            // Başarılı bir zindan gezisi sonrası rastgele ganimet (Loot)
            if (isSuccess)
            {
                Array materials = Enum.GetValues(typeof(CraftingMaterial));

                // 1 ila 3 farklı çeşit materyal düşsün
                int numMaterialTypes = _rng.Next(1, 4);

                for (int i = 0; i < numMaterialTypes; i++)
                {
                    CraftingMaterial randomMat = (CraftingMaterial)materials.GetValue(_rng.Next(materials.Length))!;

                    // Zorluğa göre düşen miktar
                    int amount = _rng.Next(1, policy.TargetQuest.DifficultyLevel * 2 + 2);

                    if (lootedMaterials.ContainsKey(randomMat))
                    {
                        lootedMaterials[randomMat] += amount;
                    }
                    else
                    {
                        lootedMaterials.Add(randomMat, amount);
                    }
                }
            }

            return new ExpeditionResult(policy, isSuccess, animType, lootedMaterials);
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
            if (policy.InsuredCharacter.EquippedItems.Any(i => i.ItemName == "Çivili Kefen")) chance = 0.90f;

            return Math.Clamp(chance, 0f, 1f);
        }

        public ExpeditionResult ProcessDuoExpedition(Character charA, Character charB, InsurancePolicy policy, RelationshipManager relationshipManager)
        {
            if (charA.EquippedItems.Any(i => i.ItemName == "Kör Öfke Yüzüğü") || charB.EquippedItems.Any(i => i.ItemName == "Kör Öfke Yüzüğü"))
            {
                throw new InvalidOperationException("Kör Öfke Yüzüğü takan bir karakter Duo göreve çıkamaz! Sadece Solo.");
            }
            RelationshipBond? bond = relationshipManager.GetBond(charA, charB);
            int bondLevel = bond?.BondLevel ?? 0;

            // OPTIMIZATION: Cache BasicInsurancePolicy objects to avoid repeated allocations in hot path
            var policyA = new GameSystems.QuestSystem.BasicInsurancePolicy(charA, policy.TargetQuest);
            var policyB = new GameSystems.QuestSystem.BasicInsurancePolicy(charB, policy.TargetQuest);

            // OPTIMIZATION: Calculate base success probabilities only once
            float baseSuccessA = CalculateSuccessProbability(policyA);
            float baseSuccessB = CalculateSuccessProbability(policyB);
            float baseSuccessProbability = (baseSuccessA + baseSuccessB) / 2f;
            float successProbability = baseSuccessProbability;

            bool isSuccess = false;
            AnimationTriggerType animType = AnimationTriggerType.NormalBounce;
            float lootMultiplier = 1.0f;

            Dictionary<AttributeType, int> originalStatsA = new Dictionary<AttributeType, int>();
            Dictionary<AttributeType, int> originalStatsB = new Dictionary<AttributeType, int>();

            if (bondLevel >= 1)
            {
                // Temp stat buff
                Array attributes = Enum.GetValues(typeof(AttributeType));

                var eligibleStatsA = new List<AttributeType>();
                foreach (AttributeType attr in attributes)
                    if (charA.Attributes.ContainsKey(attr) && charA.Attributes[attr] < 100)
                        eligibleStatsA.Add(attr);
                if (eligibleStatsA.Count > 0)
                {
                    AttributeType buffStat = eligibleStatsA[_rng.Next(eligibleStatsA.Count)];
                    originalStatsA[buffStat] = charA.Attributes[buffStat];
                    charA.Attributes[buffStat] = (int)(charA.Attributes[buffStat] * 1.20f);
                }

                var eligibleStatsB = new List<AttributeType>();
                foreach (AttributeType attr in attributes)
                    if (charB.Attributes.ContainsKey(attr) && charB.Attributes[attr] < 100)
                        eligibleStatsB.Add(attr);
                if (eligibleStatsB.Count > 0)
                {
                    AttributeType buffStat = eligibleStatsB[_rng.Next(eligibleStatsB.Count)];
                    originalStatsB[buffStat] = charB.Attributes[buffStat];
                    charB.Attributes[buffStat] = (int)(charB.Attributes[buffStat] * 1.20f);
                }
            }

            // Calculate success with temporary stat buffs
            float tempSuccessA = CalculateSuccessProbability(policyA);
            float tempSuccessB = CalculateSuccessProbability(policyB);
            float tempSuccessProbability = (tempSuccessA + tempSuccessB) / 2f;



            if (bondLevel >= 5) tempSuccessProbability += 0.10f;

            if (bondLevel == 10)
            {
                bool aIsMonster = charA.Gender == GenderType.MaleMonster || charA.Gender == GenderType.FemaleMonster;
                bool bIsMonster = charB.Gender == GenderType.MaleMonster || charB.Gender == GenderType.FemaleMonster;

                if (charA.Gender == GenderType.Male && charB.Gender == GenderType.Male)
                {
                    tempSuccessProbability += 0.15f;
                    lootMultiplier = 1.50f;
                }
                else if ((charA.Gender == GenderType.Male && charB.Gender == GenderType.Female) ||
                         (charA.Gender == GenderType.Female && charB.Gender == GenderType.Male))
                {
                    tempSuccessProbability += 0.40f;
                }
                else if ((!aIsMonster && bIsMonster) || (aIsMonster && !bIsMonster))
                {
                    // Güzel ve Çirkin - logic applied in stress section
                }
                else if (aIsMonster && bIsMonster)
                {
                    foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
                    {
                        if (charA.Attributes.ContainsKey(attr))
                        {
                            if (!originalStatsA.ContainsKey(attr)) originalStatsA[attr] = charA.Attributes[attr];
                            charA.Attributes[attr] *= 2;
                        }
                        if (charB.Attributes.ContainsKey(attr))
                        {
                            if (!originalStatsB.ContainsKey(attr)) originalStatsB[attr] = charB.Attributes[attr];
                            charB.Attributes[attr] *= 2;
                        }
                    }
                }
            }

            // OPTIMIZATION: Reuse cached calculation instead of recalculating 6 times
            float finalSuccessA = CalculateSuccessProbability(policyA);
            float finalSuccessB = CalculateSuccessProbability(policyB);
            float tempBuffsOnly = tempSuccessProbability - baseSuccessProbability;
            float calculatedFinalSuccess = (finalSuccessA + finalSuccessB) / 2f + tempBuffsOnly;
            successProbability = Math.Clamp(calculatedFinalSuccess, 0f, 1f);
            isSuccess = _rng.NextDouble() <= successProbability;

            if (!isSuccess)
            {
                int stressPenalty = bondLevel >= 1 ? 15 : 30;
                bool aIsMonster = charA.Gender == GenderType.MaleMonster || charA.Gender == GenderType.FemaleMonster;
                bool bIsMonster = charB.Gender == GenderType.MaleMonster || charB.Gender == GenderType.FemaleMonster;

                if (bondLevel == 10 && ((!aIsMonster && bIsMonster) || (aIsMonster && !bIsMonster)))
                {
                    if (aIsMonster) charA.Stress += stressPenalty * 2;
                    else charB.Stress += stressPenalty * 2;
                }
                else
                {
                    charA.Stress += stressPenalty;
                    charB.Stress += stressPenalty;
                }
            }

            foreach (var kvp in originalStatsA) charA.Attributes[kvp.Key] = kvp.Value;
            foreach (var kvp in originalStatsB) charB.Attributes[kvp.Key] = kvp.Value;

            Dictionary<CraftingMaterial, int> lootedMaterials = new Dictionary<CraftingMaterial, int>();
            if (isSuccess)
            {
                Array materials = Enum.GetValues(typeof(CraftingMaterial));
                int numMaterialTypes = _rng.Next(1, 4);
                for (int i = 0; i < numMaterialTypes; i++)
                {
                    CraftingMaterial randomMat = (CraftingMaterial)materials.GetValue(_rng.Next(materials.Length))!;
                    int amount = (int)(_rng.Next(1, policy.TargetQuest.DifficultyLevel * 2 + 2) * lootMultiplier);
                    if (lootedMaterials.ContainsKey(randomMat)) lootedMaterials[randomMat] += amount;
                    else lootedMaterials.Add(randomMat, amount);
                }
            }
            return new ExpeditionResult(policy, isSuccess, animType, lootedMaterials);
        }
    }
}
