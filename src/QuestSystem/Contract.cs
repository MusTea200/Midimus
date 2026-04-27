using System;
using GameSystems.CharacterSystem;
using GameSystems.RealEstateSystem;

namespace GameSystems.QuestSystem
{
    public class Contract
    {
        public Character Client { get; private set; }
        public DungeonProperty TargetDungeon { get; private set; }
        public int Premium { get; private set; }
        public int Payout { get; set; }
        public bool IsActive { get; set; }

        public Contract(Character client, DungeonProperty targetDungeon, int premium, int payout)
        {
            Client = client;
            TargetDungeon = targetDungeon;
            Premium = premium;
            Payout = payout;
            IsActive = true;
        }
    }
}
