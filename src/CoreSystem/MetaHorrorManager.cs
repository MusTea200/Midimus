using System;

namespace GameSystems.CoreSystem
{
    public class MetaHorrorManager
    {
        private SimulationManager _simManager;
        private Random _rng;

        public event Action? OnFakeCrashToDesktop;
        public event Action? OnFlashFakeTerminal;
        public event Action? OnCorruptSaveFileIllusion;

        public MetaHorrorManager(SimulationManager simManager)
        {
            _simManager = simManager;
            _rng = new Random();
        }

        public void TickHorrorEvents()
        {
            int karma = _simManager.HiddenKarmaScore;

            if (karma <= -100)
            {
                double roll = _rng.NextDouble();

                if (roll <= 0.01) // %1 ihtimalle sahte çökme
                {
                    System.Console.WriteLine("[META] Oyun çökmüş gibi yapıyor. Sahte Masaüstü tetikleniyor...");
                    OnFakeCrashToDesktop?.Invoke();
                }
                else if (roll <= 0.03) // (0.01 ile 0.03 arası) %2 ihtimalle sahte terminal
                {
                    System.Console.WriteLine("[META] Ekranda milisaniyelik sahte bir terminal belirdi.");
                    OnFlashFakeTerminal?.Invoke();
                }
                else if (roll <= 0.035) // Binde 5 ihtimalle sahte save silinmesi illüzyonu
                {
                    System.Console.WriteLine("[META] 'Kayıt dosyası silindi' illüzyonu tetiklendi.");
                    OnCorruptSaveFileIllusion?.Invoke();
                }
            }
        }
    }
}
