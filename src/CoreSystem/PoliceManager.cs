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

        public void TriggerPoliceInterrogation(GameSystems.CharacterSystem.Character character, DailyLedger ledger)
        {
            System.Console.WriteLine($"DÜDÜK SESİ! Polis {character.Name}'nin yasa dışı yeni bir bedende dolaştığından şüphelendi ve sorguya çekti.");

            System.Random rng = new System.Random();
            int charisma = character.Attributes.ContainsKey(GameSystems.CharacterSystem.AttributeType.Charisma) ? character.Attributes[GameSystems.CharacterSystem.AttributeType.Charisma] : 10;

            // Rüşvetçi vs pasifler varsa eklenebilir, şimdilik direkt Charisma zar atalım
            if (rng.Next(1, 101) <= charisma)
            {
                int bribeCost = 100;
                if (ledger.MainBalance >= bribeCost)
                {
                    ledger.DeductBalance(bribeCost);
                    System.Console.WriteLine($"BAŞARILI! {character.Name} polislere rüşvet verdi ve tatlı diliyle ikna etti. (-100 Altın)");
                }
                else
                {
                    ledger.DeductBalance(ledger.MainBalance);
                    System.Console.WriteLine($"BAŞARILI sayılır. {character.Name} polisleri ikna etti ama cebindeki tüm altını verdiler.");
                }
            }
            else
            {
                int penalty = 5000;
                if (ledger.MainBalance >= penalty) ledger.DeductBalance(penalty);
                else ledger.DeductBalance(ledger.MainBalance);

                System.Console.WriteLine($"BAŞARISIZ! Yasadışı zihin aktarımı suçu tespit edildi.");
                System.Console.WriteLine($"{character.Name} tutuklandı ve sistemden silindi! Şirkete büyük bir ceza (-5000 Altın) kesildi.");

                character.Traits.Clear();
                character.AdvancedTraits.Clear();
                character.Stress = 100;
                // Oyun motoru bu karakteri ölü/tutuklu olarak algılayıp listeden çıkartmalı.
            }
        }
}
}
