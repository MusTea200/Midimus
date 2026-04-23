using System.Collections.Generic;
using System.Linq;

namespace GameSystems.CharacterSystem
{
    public class RelationshipManager
    {
        private List<RelationshipBond> _bonds;

        public RelationshipManager()
        {
            _bonds = new List<RelationshipBond>();
        }

        public RelationshipBond GetOrCreateBond(Character charA, Character charB)
        {
            var existingBond = _bonds.FirstOrDefault(b => b.Contains(charA, charB));
            if (existingBond != null)
            {
                return existingBond;
            }

            var newBond = new RelationshipBond(charA, charB);
            _bonds.Add(newBond);
            return newBond;
        }

        public RelationshipBond? GetBond(Character charA, Character charB)
        {
            return _bonds.FirstOrDefault(b => b.Contains(charA, charB));
        }
    }
}
