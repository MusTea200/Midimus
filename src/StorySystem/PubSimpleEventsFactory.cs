using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.StorySystem
{
    public static class PubSimpleEventsFactory
    {
        private static Random _rng = new Random();

        // 1. Sarhoş Maceracı (Drunk Adventurer)
        public static void TriggerDrunkAdventurer(Character character, DailyLedger ledger, PlayerManager playerManager)
        {
            Console.WriteLine("Sarhoş Maceracı: Bir maceracı masana yanaştı ve saçmalamaya başladı.");
            switch (character.Gender)
            {
                case GenderType.Male:
                case GenderType.Female:
                    int agility = character.Attributes.ContainsKey(AttributeType.Agility) ? character.Attributes[AttributeType.Agility] : 10;
                    if (_rng.Next(1, 101) <= agility)
                    {
                        ledger.AddBalance(100);
                        Console.WriteLine($"Başarılı! {character.Name} adamın cebinden 100 Altın aşırdı.");
                    }
                    else
                    {
                        character.Stress += 20;
                        Console.WriteLine($"Başarısız! {character.Name} yakalandı ve kavga çıktı. Stres arttı (+20).");
                    }
                    break;
                case GenderType.MaleMonster:
                case GenderType.FemaleMonster:
                    Console.WriteLine($"Adam {character.Name}'den korkup kaçtı ve çantasını düşürdü.");
                    Array materials = Enum.GetValues(typeof(CraftingMaterial));
                    CraftingMaterial randomMat = (CraftingMaterial)materials.GetValue(_rng.Next(materials.Length))!;
                    playerManager.AddMaterial(randomMat, 1);
                    Console.WriteLine($"{character.Name} çantadan 1 adet {randomMat} buldu!");
                    break;
            }
        }

        // 2. Han Ozanı (Tavern Bard)
        public static void TriggerTavernBard(Character character, DailyLedger ledger)
        {
            Console.WriteLine("Han Ozanı: Ozan etkileyici bir şarkıya başladı.");
            switch (character.Gender)
            {
                case GenderType.Male:
                case GenderType.Female:
                    character.Exp += 50;
                    Console.WriteLine($"{character.Name} şarkıdan ilham aldı. +50 EXP kazandı.");
                    break;
                case GenderType.FemaleMonster:
                    character.Stress = 0;
                    Console.WriteLine($"{character.Name} şarkının ritmine kapıldı. Stresi tamamen sıfırlandı.");
                    break;
                case GenderType.MaleMonster:
                    ledger.AddBalance(50);
                    Console.WriteLine($"Ozan, {character.Name}'nin korkutucu bakışlarından tırsıp ona 50 Altın haraç verdi.");
                    break;
            }
        }

        // 3. Gizemli İksir (Mysterious Brew)
        public static void TriggerMysteriousBrew(Character character)
        {
            Console.WriteLine("Gizemli İksir: Barmen sana rengi sürekli değişen bir içki ikram etti.");
            switch (character.Gender)
            {
                case GenderType.Male:
                case GenderType.Female:
                    character.Stress += 30;
                    character.Exp += 100;
                    Console.WriteLine($"{character.Name} iksiri içti! Midesi yandı (Stres +30) ama vizyonlar gördü (+100 EXP).");
                    break;
                case GenderType.MaleMonster:
                case GenderType.FemaleMonster:
                    character.Stress = 0;
                    Console.WriteLine($"{character.Name} bu zehirli karışımı çok sevdi. Stresi sıfırlandı!");
                    break;
            }
        }

        // 4. Bilek Güreşi (Arm Wrestling)
        public static void TriggerArmWrestling(Character character, DailyLedger ledger)
        {
            Console.WriteLine("Bilek Güreşi: İriyarı bir adam masaya yumruğunu vurdu ve meydan okudu!");
            int strength = character.Attributes.ContainsKey(AttributeType.Strength) ? character.Attributes[AttributeType.Strength] : 10;

            switch (character.Gender)
            {
                case GenderType.Male:
                    if (_rng.Next(1, 101) <= strength)
                    {
                        ledger.AddBalance(150);
                        Console.WriteLine($"Başarılı! {character.Name} adamı yendi. +150 Altın.");
                    }
                    else
                    {
                        ledger.DeductBalance(50);
                        Console.WriteLine($"Başarısız! {character.Name} yenildi ve iddiayı kaybetti. -50 Altın.");
                    }
                    break;
                case GenderType.Female:
                    // Kadınlara +20 bonus şans eklenebilir veya zar kolaylaştırılabilir.
                    if (_rng.Next(1, 101) <= (strength + 20))
                    {
                        ledger.AddBalance(200);
                        Console.WriteLine($"Başarılı! Adam {character.Name}'yi hafife aldı ve kaybetti! +200 Altın.");
                    }
                    else
                    {
                        ledger.DeductBalance(50);
                        Console.WriteLine($"Başarısız! {character.Name} yenildi. -50 Altın.");
                    }
                    break;
                case GenderType.MaleMonster:
                case GenderType.FemaleMonster:
                    ledger.AddBalance(75);
                    Console.WriteLine($"Adam {character.Name}'nin kaslarını görünce pes edip masasına döndü. Çekilme payı: +75 Altın.");
                    break;
            }
        }

        // 5. Şüpheli Kutu (Suspicious Box)
        public static void TriggerSuspiciousBox(Character character, DailyLedger ledger, PlayerManager playerManager, Action<bool> onSmellResult)
        {
            Console.WriteLine("Şüpheli Kutu: Tüccar masana kilitli bir kutu koydu. 'Sadece 100 Altın.' diyor.");

            if (ledger.MainBalance < 100)
            {
                Console.WriteLine("Kutuyu alacak yeterli altının yok.");
                return;
            }

            switch (character.Gender)
            {
                case GenderType.Male:
                case GenderType.Female:
                    ledger.DeductBalance(100);
                    if (_rng.NextDouble() <= 0.5)
                    {
                        playerManager.AddMaterial(CraftingMaterial.MagicDust, 1);
                        Console.WriteLine($"Kutuyu açtın! İçinden nadir materyal 'MagicDust' çıktı.");
                    }
                    else
                    {
                        Console.WriteLine($"Kutuyu açtın! ...İçi boş. Kazıklandın.");
                    }
                    break;
                case GenderType.MaleMonster:
                case GenderType.FemaleMonster:
                    // Koku alma mekaniği: Gerçekten içinde eşya olup olmadığını önden hisseder.
                    bool hasLoot = _rng.NextDouble() <= 0.5;
                    Console.WriteLine($"{character.Name} kutuyu kokluyor...");
                    onSmellResult?.Invoke(hasLoot); // UI'a GoodSmell (true) veya BadSmell (false) yollar.
                    // Oyuncu bu sonuca göre kutuyu açıp açmamaya karar verecek (UI tarafı yönetecek).
                    break;
            }
        }
    }
}
