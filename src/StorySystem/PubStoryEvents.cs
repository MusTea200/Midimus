using System;

namespace GameSystems.StorySystem
{
    public enum BrawlResult
    {
        Victory,
        Defeat,
        Fled
    }

    public class PubBrawlConfig
    {
        public float CursorSpeed { get; private set; }
        public float GreenZoneSize { get; private set; }

        public PubBrawlConfig(int agility, int strength)
        {
            // Yüksek agility yavaş cursor demek ki oyuncu kolay yakalasın
            // Veya yüksek agility hızlı cursor ama oyuncunun tepki süresine uygun da olabilir.
            // Biz basitçe: Agility yüksekse iş kolaylaşır (hız düşer).
            CursorSpeed = Math.Max(0.5f, 5.0f - (agility / 20f));

            // Yüksek güç, vurulacak alanın büyümesini sağlar.
            GreenZoneSize = Math.Min(5.0f, 1.0f + (strength / 20f));
        }
    }
}
