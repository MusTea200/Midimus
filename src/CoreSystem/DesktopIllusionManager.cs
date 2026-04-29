using System;

namespace GameSystems.CoreSystem
{
    public class DesktopIllusionManager
    {
        private Random _rng;

        public event Action? OnFakeDesktopReady;
        public event Action? OnCreepyPopupTriggered;
        public event Action? OnDesktopBleedTriggered;
        public event Action? OnInputBlockedByEntity;

        public DesktopIllusionManager()
        {
            _rng = new Random();
        }

        public void CapturePlayerDesktop()
        {
            // Uses System.Drawing.Graphics.CopyFromScreen to capture screen bounds (Engine specific implementation)
            Console.WriteLine("[İLLÜZYON] Oyuncunun masaüstü arka planı yakalandı ve sahte bir ekran olarak renderlanıyor...");
            OnFakeDesktopReady?.Invoke();
        }

        public void ProcessFakeIconClick(int clickCount)
        {
            if (clickCount >= 2)
            {
                if (_rng.NextDouble() <= 0.50)
                {
                    OnCreepyPopupTriggered?.Invoke();
                }
                else
                {
                    OnDesktopBleedTriggered?.Invoke();
                }
            }
        }

        public void ProcessKeyboardInput()
        {
            // This is called when the user tries to press ESC or other keys to escape the fake desktop
            Console.WriteLine("[İLLÜZYON] Oyuncu kaçmaya çalıştı. Klavye girdisi varlık tarafından engellendi.");
            OnInputBlockedByEntity?.Invoke();
        }
    }
}
