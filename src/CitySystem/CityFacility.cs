using System;
using GameSystems.CharacterSystem;

namespace GameSystems.CitySystem
{
    public abstract class CityFacility
    {
        public string FacilityName { get; protected set; }
        public int Level { get; protected set; }
        public int UpgradeCost { get; protected set; }

        protected CityFacility(string name, int baseUpgradeCost)
        {
            FacilityName = name;
            Level = 1;
            UpgradeCost = baseUpgradeCost;
        }

        public virtual void UpgradeFacility()
        {
            Level++;
            UpgradeCost = (int)(UpgradeCost * 1.5f);
            Console.WriteLine($"{FacilityName} {Level}. seviyeye yükseltildi!");
        }

        public abstract void EnterFacility(Character character);
    }
}
