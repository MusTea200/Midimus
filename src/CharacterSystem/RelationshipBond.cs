using System;

namespace GameSystems.CharacterSystem
{
    public class RelationshipBond
    {
        public Character CharA { get; private set; }
        public Character CharB { get; private set; }

        private int _bondLevel;
        public int BondLevel
        {
            get => _bondLevel;
            set => _bondLevel = Math.Clamp(value, 0, 10);
        }

        public RelationshipBond(Character charA, Character charB)
        {
            CharA = charA;
            CharB = charB;
            BondLevel = 0;
        }

        public bool Contains(Character charA, Character charB)
        {
            return (CharA == charA && CharB == charB) || (CharA == charB && CharB == charA);
        }
    }
}
