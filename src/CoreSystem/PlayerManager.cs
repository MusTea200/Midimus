using System;
using System.Collections.Generic;

namespace GameSystems.CoreSystem
{
    public class PlayerManager
    {
        private int _sleightOfHand;
        public int SleightOfHand
        {
            get => _sleightOfHand;
            set => _sleightOfHand = Math.Clamp(value, 1, 100);
        }

        private int _persuasion;
        public int Persuasion
        {
            get => _persuasion;
            set => _persuasion = Math.Clamp(value, 1, 100);
        }

        public Dictionary<CraftingMaterial, int> MaterialInventory { get; private set; }

        public PlayerManager(int initialSleightOfHand = 50, int initialPersuasion = 50)
        {
            SleightOfHand = initialSleightOfHand;
            Persuasion = initialPersuasion;
            MaterialInventory = new Dictionary<CraftingMaterial, int>();

            foreach (CraftingMaterial mat in Enum.GetValues(typeof(CraftingMaterial)))
            {
                MaterialInventory[mat] = 0;
            }
        }

        public void AddMaterial(CraftingMaterial material, int amount)
        {
            if (amount > 0)
            {
                MaterialInventory[material] += amount;
            }
        }

        public bool HasMaterial(CraftingMaterial material, int amount)
        {
            return MaterialInventory.ContainsKey(material) && MaterialInventory[material] >= amount;
        }

        public bool ConsumeMaterial(CraftingMaterial material, int amount)
        {
            if (HasMaterial(material, amount))
            {
                MaterialInventory[material] -= amount;
                return true;
            }
            return false;
        }

        // Yetenek zarı atar. RNG sonucu yetenek puanından küçükse veya eşitse başarılı sayılır.
        public bool RollSleightOfHand()
        {
            Random rng = new Random();
            int roll = rng.Next(1, 101);
            return roll <= SleightOfHand;
        }

        public bool RollPersuasion()
        {
            Random rng = new Random();
            int roll = rng.Next(1, 101);
            return roll <= Persuasion;
        }
    }
}
