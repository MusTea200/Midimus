using System;

namespace GameSystems.CoreSystem
{
    public class PoliceManager
    {
        private int _wantedStars;
        public int WantedStars
        {
            get => _wantedStars;
            private set
            {
                _wantedStars = Math.Clamp(value, 0, 5);
                if (_wantedStars == 5)
                {
                    TriggerBustEvent();
                }
            }
        }

        private int _policeReputation;
        public int PoliceReputation
        {
            get => _policeReputation;
            private set
            {
                _policeReputation = Math.Clamp(value, 0, 100);
                if (_policeReputation >= 100 && !IsPoliceInsuranceUnlocked)
                {
                    IsPoliceInsuranceUnlocked = true;
                    Console.WriteLine("Polis departmanıyla ilişkiniz maksimuma ulaştı. 'Polis Sigortası' kilidi açıldı!");
                }
            }
        }

        public bool IsPoliceInsuranceUnlocked { get; private set; }

        public event Action? OnBustEventTriggered;

        public PoliceManager()
        {
            WantedStars = 0;
            PoliceReputation = 0;
            IsPoliceInsuranceUnlocked = false;
        }

        public void CommitIllegalAction(int severity)
        {
            // Ciddiyetine göre yıldız artırır (Örn: 1-2 yıldız)
            Console.WriteLine($"İllegal bir eylem yapıldı! Aranma seviyesi artıyor...");
            WantedStars += severity;
        }

        public void BribePolice(int bribeAmount)
        {
            // Paraya göre aranma düşer veya repütasyon artar
            if (WantedStars > 0)
            {
                WantedStars -= 1;
                Console.WriteLine("Polislere rüşvet verildi. Aranma seviyesi düştü.");
            }
            else
            {
                PoliceReputation += 10;
                Console.WriteLine("Polislere donat ikram edildi. Repütasyon arttı.");
            }
        }

        public void SnitchToPolice()
        {
            // Muhbirlik yapmak büyük repütasyon sağlar
            PoliceReputation += 25;
            Console.WriteLine("Polise muhbirlik yapıldı. Repütasyon ciddi şekilde arttı.");
        }

        private void TriggerBustEvent()
        {
            Console.WriteLine("DİKKAT! Aranma seviyesi 5 Yıldıza ulaştı. POLİS BASKINI!");
            OnBustEventTriggered?.Invoke();
            // Baskın sonrası aranma seviyesi sıfırlanabilir veya oyun bitebilir.
            WantedStars = 0;
        }
    }
}
