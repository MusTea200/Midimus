using System;

namespace GameSystems.CoreSystem
{
    public class GamePhaseManager
    {
        public bool IsCityPhaseUnlocked { get; private set; }

        public GamePhaseManager()
        {
            IsCityPhaseUnlocked = false;
        }

        // Oyuncu yeterli zenginliğe ulaşınca veya belirli bir hikaye noktasında çağrılır
        public void UnlockCityPhase()
        {
            if (!IsCityPhaseUnlocked)
            {
                IsCityPhaseUnlocked = true;
                Console.WriteLine("Şehir Fazı (City Phase) açıldı! Artık yeni tesislere erişebilirsiniz.");
            }
        }
    }
}
