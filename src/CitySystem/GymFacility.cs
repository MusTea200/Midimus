using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class GymFacility : TrainingFacility
    {
        private static readonly List<AttributeType> _trainableStats = new List<AttributeType>
        {
            AttributeType.Strength,
            AttributeType.Agility
        };

        protected override List<AttributeType> TrainableStats => _trainableStats;

        public GymFacility(DailyLedger ledger)
            : base("Spor Salonu", 1000, ledger, 150, 20)
        {
        }
    }
}
