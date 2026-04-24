using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public abstract class TrainingFacility : CityFacility
    {
        protected DailyLedger Ledger { get; private set; }
        public int TrainingGoldCost { get; protected set; }
        public int TrainingStressCost { get; protected set; }

        protected abstract List<AttributeType> TrainableStats { get; }

        protected TrainingFacility(string name, int baseUpgradeCost, DailyLedger ledger, int trainingGoldCost, int trainingStressCost)
            : base(name, baseUpgradeCost)
        {
            Ledger = ledger;
            TrainingGoldCost = trainingGoldCost;
            TrainingStressCost = trainingStressCost;
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name}, {FacilityName} tesisine giriş yaptı.");
        }

        public virtual bool TrainCharacter(Character character, AttributeType statToTrain)
        {
            if (!TrainableStats.Contains(statToTrain))
            {
                Console.WriteLine($"{FacilityName} tesisinde {statToTrain} statüsü eğitilemez.");
                return false;
            }

            if (character.Stress > 80)
            {
                Console.WriteLine($"{character.Name} çok stresli (Stres: {character.Stress}). Eğitime alınamaz!");
                return false;
            }

            if (Ledger.MainBalance < TrainingGoldCost)
            {
                Console.WriteLine($"Eğitim için yeterli altın yok. Gereken: {TrainingGoldCost}, Mevcut: {Ledger.MainBalance}");
                return false;
            }

            Ledger.DeductBalance(TrainingGoldCost);
            character.Stress += TrainingStressCost;

            if (character.Attributes.ContainsKey(statToTrain))
            {
                character.Attributes[statToTrain] += 5; // Fixed increase for now, can be scaled with level

                // Cap stat at 100
                if (character.Attributes[statToTrain] > 100)
                {
                     character.Attributes[statToTrain] = 100;
                }
            }
            else
            {
                character.Attributes[statToTrain] = 5;
            }

            Console.WriteLine($"{character.Name} başarıyla eğitildi! {statToTrain} arttı. Yeni Stres: {character.Stress}");
            return true;
        }
    }
}
