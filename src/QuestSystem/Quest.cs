using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;

namespace GameSystems.QuestSystem
{
    public class Quest
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int DifficultyLevel { get; private set; }
        public Dictionary<AttributeType, int> Requirements { get; private set; }

        public int BaseReward { get; private set; }
        public int BaseDanger { get; private set; } // Başarısızlık/Ölüm tehlikesi baz puanı

        public Quest(string name, int difficulty, int reward, int danger)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            DifficultyLevel = difficulty;
            BaseReward = reward;
            BaseDanger = danger;
            Requirements = new Dictionary<AttributeType, int>();
        }
    }
}
