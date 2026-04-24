using System;
using System.Collections.Generic;

namespace GameSystems.CharacterSystem
{
    public class BerserkCurseTrait : UpgradeableTrait
    {
        public int ExpeditionCount { get; private set; }
        public bool IsLocked { get; private set; }

        public BerserkCurseTrait() : base("Berserker's Shroud Curse")
        {
            ExpeditionCount = 0;
            IsLocked = false;
        }

        public void IncrementExpedition(Character character)
        {
            if (IsLocked && ExpeditionCount >= 15)
            {
                // 16. görev: Kalıcı Ölüm
                ExpeditionCount++;
                Console.WriteLine($"[PERMADEATH] {character.Name} karanlığa karıştı ve bir daha geri dönmedi...");
                // Oyun motoru karakteri silecek (Örn: Event fırlatılabilir veya bir flag konulabilir).
                // Şimdilik sadece log atıyoruz.
                return;
            }

            ExpeditionCount++;
            ApplyThresholds(character);
        }

        private void ApplyThresholds(Character character)
        {
            // Statları geri yükleyip tekrar düşürme gibi kompleks işlemleri basitleştirmek için
            // her seferinde güncel ceza miktarını hesaplıyoruz.
            // (Gerçek bir projede BaseAttributes ve CurrentAttributes ayrımı olurdu).

            if (ExpeditionCount == 10)
            {
                Console.WriteLine($"{character.Name} için stat düşüşü kalıcı %50'ye ulaştı. Rastgele debuff eklendi.");
                character.Traits.Add(TraitType.Cowardly); // Rastgele debuff yerine örnek veriyoruz
            }
            else if (ExpeditionCount == 15)
            {
                Console.WriteLine($"{character.Name} için stat düşüşü kalıcı %90'a ulaştı. Lanet mühürlendi (Geri Döndürülemez)!");
                character.Traits.Add(TraitType.Irritable);
                character.Traits.Add(TraitType.Cautious);
                IsLocked = true;
            }
        }
    }
}
