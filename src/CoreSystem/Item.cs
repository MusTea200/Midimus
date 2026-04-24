using System;

namespace GameSystems.CoreSystem
{
    public class Item
    {
        public string ItemName { get; private set; }
        public bool IsUnidentified { get; set; }
        public bool IsBroken { get; set; }
        public bool IsCursed { get; set; }
        public bool IsBound { get; set; }

        public int BaseStatValue { get; private set; }
        public int RevealedStatValue { get; set; }
        public int ElementalDamage { get; set; }

        public Item(string name, int baseStatValue)
        {
            ItemName = name;
            BaseStatValue = baseStatValue;
            IsUnidentified = true;
            IsBroken = false;
            IsCursed = false;
            IsBound = false;
            RevealedStatValue = 0;
            ElementalDamage = 0;
        }
    }
}
