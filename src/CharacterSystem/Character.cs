using System;
using System.Collections.Generic;

namespace GameSystems.CharacterSystem
{
    public enum AttributeType
    {
        Strength, Agility, Intelligence, Luck, Endurance, Charisma, Perception, Willpower
    }

    public enum TraitType
    {
        Brave, Cowardly, Greedy, Generous, Gluttonous, Irritable
    }

    public enum InteractionType
    {
        ForceDiet, HardTraining, ForceSocialize, OfferRest, GiveFavoriteFood
    }

    public class Character
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public Dictionary<AttributeType, int> Attributes { get; private set; } // 1-100 arası
        public List<TraitType> Traits { get; private set; }
        public Dictionary<InteractionType, bool> RedLines { get; private set; } // True: Kırmızı çizgi, False: Sevdiği eylem

        private int _stress;
        public int Stress
        {
            get => _stress;
            private set => _stress = Math.Clamp(value, 0, 100);
        }

        // Uyum seviyesi: Stres arttıkça oyuncunun kararlarına itaati düşer (0.0f - 1.0f)
        public float Compliance => 1.0f - (Stress / 100f);

        public Character(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Attributes = new Dictionary<AttributeType, int>();
            Traits = new List<TraitType>();
            RedLines = new Dictionary<InteractionType, bool>();
            Stress = 0;

            // Yetenekleri varsayılan olarak başlat
            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
                Attributes[attr] = new Random().Next(10, 50);
        }

        // Oyuncunun karakterle girdiği etkileşimi çözümleyen fonksiyon
        public void ApplyInteraction(InteractionType interaction)
        {
            if (RedLines.TryGetValue(interaction, out bool isRedLine))
            {
                if (isRedLine) // Karakterin kırmızı çizgisine basıldı (Örn: Obura zorla diyet yaptırmak)
                {
                    Stress += 30;
                    HandleAggressiveBehavior();
                }
                else // Karakterin sevdiği eylem yapıldı
                {
                    Stress -= 20;
                }
            }
        }

        private void HandleAggressiveBehavior()
        {
            if (Stress > 70)
            {
                // Rastgele karar tetikleyicisi (Örn: Göreve gitmeyi reddetme, eşya kırma, zam isteme)
                Console.WriteLine($"{Name} çok stresli! Otoriteye karşı geliyor...");
            }
        }
    }
}
