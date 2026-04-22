using System;
using GameSystems.CharacterSystem;

namespace GameSystems.QuestSystem
{
    public class EconomyPolicy : InsurancePolicy
    {
        public EconomyPolicy(Character character, Quest quest) : base(character, quest) { }

        protected override void CalculatePolicy()
        {
            base.CalculatePolicy(); // Temel hesaplamayı yap

            // Tutumlu karakterler ucuz teklifleri sever
            Premium = (int)(Premium * 0.7f);
            Compensation = (int)(Compensation * 0.5f);
        }

        public override void ApplyPolicyEffects()
        {
            if (_effectsApplied) return;
            base.ApplyPolicyEffects();

            if (InsuredCharacter.Traits.Contains(TraitType.Thrifty))
            {
                InsuredCharacter.IdealOfferRatio += 0.1f; // Teklifi kabul etme ihtimali artar
            }
        }
    }

    public class PetInsurancePolicy : InsurancePolicy
    {
        public PetInsurancePolicy(Character character, Quest quest) : base(character, quest) { }

        protected override void CalculatePolicy()
        {
            base.CalculatePolicy();

            Premium = (int)(Premium * 1.1f);
            Compensation = (int)(Compensation * 1.2f);
        }

        public override void ApplyPolicyEffects()
        {
            if (_effectsApplied) return;
            base.ApplyPolicyEffects();

            if (InsuredCharacter.Traits.Contains(TraitType.HasPet))
            {
                InsuredCharacter.Patience = Math.Min(5, InsuredCharacter.Patience + 1); // Evcil hayvanı olanlar bu teklife sıcak bakar
                InsuredCharacter.IdealOfferRatio += 0.15f;
            }
        }
    }

    public class RetirementPolicy : InsurancePolicy
    {
        public RetirementPolicy(Character character, Quest quest) : base(character, quest) { }

        protected override void CalculatePolicy()
        {
            base.CalculatePolicy();

            Premium = (int)(Premium * 1.3f);
            Compensation = (int)(Compensation * 2.0f); // Yüksek tazminat garantisi
        }

        public override void ApplyPolicyEffects()
        {
            if (_effectsApplied) return;
            base.ApplyPolicyEffects();

            if (InsuredCharacter.Traits.Contains(TraitType.Cautious))
            {
                InsuredCharacter.IdealOfferRatio += 0.2f; // Garantici karakterler bayılır
            }
        }
    }

    public class ComboTaskPolicy : InsurancePolicy
    {
        public ComboTaskPolicy(Character character, Quest quest) : base(character, quest) { }

        protected override void CalculatePolicy()
        {
            base.CalculatePolicy();

            // Ganimetin belirli bir kısmı (örneğin görev baz ödülünün 1/3'ü) prim olarak eklenir
            int extraLootShare = TargetQuest.BaseReward / 3;
            Premium += extraLootShare;
        }

        public override void ApplyPolicyEffects()
        {
            if (_effectsApplied) return;
            base.ApplyPolicyEffects();

            // Agresif/Cesur görevlere uygun olan karakterler bu tarzı tercih edebilir
            if (InsuredCharacter.Traits.Contains(TraitType.Brave))
            {
                InsuredCharacter.IdealOfferRatio += 0.1f;
            }
            else
            {
                // Cesur olmayanlar için bu teklif daha stres vericidir
                InsuredCharacter.Stress += 10;
            }
        }
    }
}
