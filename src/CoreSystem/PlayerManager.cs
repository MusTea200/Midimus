using System;

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

        public PlayerManager(int initialSleightOfHand = 50, int initialPersuasion = 50)
        {
            SleightOfHand = initialSleightOfHand;
            Persuasion = initialPersuasion;
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
