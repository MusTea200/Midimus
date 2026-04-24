using System;
using System.Collections.Generic;

namespace GameSystems.CoreSystem
{
    public class DailyLedger
    {
        public int MainBalance { get; private set; }
        private Queue<ExpeditionResult> _pendingLedgerItems;

        // UI için eventler
        public event Action<ExpeditionResult, int>? OnLedgerItemProcessed; // (İşlenen öğe, Güncel Bakiye)
        public event Action<int>? OnDailyLedgerCompleted; // Gün sonu toplam bakiye

        public DailyLedger(int startingBalance)
        {
            MainBalance = startingBalance;
            _pendingLedgerItems = new Queue<ExpeditionResult>();
        }

        public void DeductBalance(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Tutar negatif olamaz.");
            MainBalance -= amount;
        }

        public void AddBalance(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Tutar negatif olamaz.");
            MainBalance += amount;
        }

        public void LoadDailyResults(List<ExpeditionResult> dailyResults)
        {
            _pendingLedgerItems.Clear();
            foreach (var result in dailyResults)
            {
                _pendingLedgerItems.Enqueue(result);
            }
        }

        // UI tarafından, listeyi satır satır ve animasyonlu yazdırmak için çağrılır
        public bool ProcessNextLedgerItem()
        {
            if (_pendingLedgerItems.Count > 0)
            {
                ExpeditionResult result = _pendingLedgerItems.Dequeue();

                // Bakiyeyi güncelle (Kazanıldıysa prim eklenir, ölündüyse tazminat düşülür)
                MainBalance += result.LedgerAmount;

                // UI'ı bilgilendir
                OnLedgerItemProcessed?.Invoke(result, MainBalance);

                return true; // Hala işlenecek kalem var veya son kalem işlendi
            }

            return false; // Liste boş
        }

        // Gün sonunda emlak gelirleri ve giderlerini topluca ledger'a yansıt
        public void ApplyRealEstateEconomics(int dailyCommissions, int dailyMaintenanceCosts)
        {
            MainBalance += dailyCommissions;
            MainBalance -= dailyMaintenanceCosts;
            Console.WriteLine($"Emlak Bilançosu: +{dailyCommissions} Gelir, -{dailyMaintenanceCosts} Gider. Yeni Bakiye: {MainBalance}");
        }

        public void NextDay()
        {
            _pendingLedgerItems.Clear();
            OnDailyLedgerCompleted?.Invoke(MainBalance);
            // Ertesi güne hazırlık işlemleri (Örn: Gider kesintileri vs. eklenebilir)
        }
    }
}
