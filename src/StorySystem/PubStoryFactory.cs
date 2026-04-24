using System;
using System.Collections.Generic;
using GameSystems.CoreSystem;
using GameSystems.CharacterSystem;
using GameSystems.CitySystem;

namespace GameSystems.StorySystem
{
    public static class PubStoryFactory
    {
        // Mini Game Events UI'ın abone olması için
        public static event Action<int, Action<bool>>? OnDrinkingContestStarted;
        public static event Action<PubBrawlConfig, Action<BrawlResult>>? OnPubBrawlTriggered;

        public static void RegisterPubEvents(StoryManager manager, DailyLedger ledger, Character character, RelationshipManager relManager, JobCenterFacility jobCenter)
        {
            Random rng = new Random();
            double roll = rng.NextDouble();

            // Sadece biri tetiklensin mantığı yapabiliriz
            if (roll <= 0.02) // %2
            {
                CreateLoveOfMyLifeNode(manager, character, relManager, jobCenter);
                manager.TriggerNode("Love_Of_My_Life");
            }
            else if (roll <= 0.04) // %2 (0.02 ile 0.04 arası)
            {
                TriggerPubBrawl(character, ledger);
            }
            else if (roll <= 0.09) // %5 (0.04 ile 0.09 arası)
            {
                CreateHolyBeerNode(manager, character);
                manager.TriggerNode("Holy_Beer_Start");
            }

            // Haftalık event dışarıdan çağrılmalı, burada bir örnek fonksiyon sunuyoruz.
        }

        // 1. Kutsal Bira (The Holy Beer - Easter Egg)
        public static void CreateHolyBeerNode(StoryManager manager, Character character)
        {
            GlobalStoryState state = manager.GlobalState;

            StoryNode nodeStart = new StoryNode(
                id: "Holy_Beer_Start",
                title: "Kutsal Bira Dedikodusu",
                description: "Pub'da efsanevi bir biranın saklandığı dedikodusunu duydun."
            );

            nodeStart.AddChoice(new StoryChoice(
                text: "Çatıya Çık",
                onSelected: () => manager.TriggerNode("Holy_Beer_Roof")
            ));

            nodeStart.AddChoice(new StoryChoice(
                text: "Sallamasyon dedikodu deyip geç",
                onSelected: () => Console.WriteLine("Biranı içip dedikoduyu umursamadın.")
            ));

            manager.RegisterNode(nodeStart);

            StoryNode nodeRoof = new StoryNode(
                id: "Holy_Beer_Roof",
                title: "Çatıda",
                description: "Çatıdasın. Rüzgar esiyor ve bacadan garip bir koku geliyor."
            );

            nodeRoof.AddChoice(new StoryChoice(
                text: "Bacayı İncele",
                onSelected: () =>
                {
                    Console.WriteLine("Bacanın yanında beyaz bir sakal peruğu ve parlayan efsanevi bir şişe buldun. Noel Baba burada mıydı?");
                    character.Stress = 0;
                    state.AddFlag("Found_Holy_Beer");
                    Console.WriteLine($"{character.Name} Kutsal Birayı içti ve stresi tamamen sıfırlandı!");
                }
            ));

            manager.RegisterNode(nodeRoof);
        }

        // 2. Hayatımın Aşkı (Love of My Life)
        public static void CreateLoveOfMyLifeNode(StoryManager manager, Character character, RelationshipManager relManager, JobCenterFacility jobCenter)
        {
            StoryNode node = new StoryNode(
                id: "Love_Of_My_Life",
                title: "Hayatımın Aşkı",
                description: "Barda inanılmaz çekici biri oturuyor. Göz göze geldiniz. Ne yapacaksın?"
            );

            // Charisma Check
            node.AddChoice(new StoryChoice(
                text: "Gülümseyip yanına git ve iltifat et. (Charisma Check)",
                onSelected: () => ResolveLoveCheck(character, AttributeType.Charisma, relManager, jobCenter)
            ));

            // Intelligence Check
            node.AddChoice(new StoryChoice(
                text: "İlginç bir edebi alıntıyla söze gir. (Intelligence Check)",
                onSelected: () => ResolveLoveCheck(character, AttributeType.Intelligence, relManager, jobCenter)
            ));

            // Strength Check
            node.AddChoice(new StoryChoice(
                text: "Bardağını tek dikişte bitirip gücünü göstererek yaklaş. (Strength Check)",
                onSelected: () => ResolveLoveCheck(character, AttributeType.Strength, relManager, jobCenter)
            ));

            manager.RegisterNode(node);
        }

        private static void ResolveLoveCheck(Character character, AttributeType requiredStat, RelationshipManager relManager, JobCenterFacility jobCenter)
        {
            Random rng = new Random();
            int charStat = character.Attributes.ContainsKey(requiredStat) ? character.Attributes[requiredStat] : 10;

            // 1-100 arası zar atılıyor, stat ne kadar yüksekse şans o kadar çok
            if (rng.Next(1, 101) <= charStat)
            {
                // Başarı: Partner oluştur
                Character partner = new Character($"Gizemli Aşık {rng.Next(100,999)}");
                jobCenter.HeroPool.Add(partner); // Sisteme dahil et

                RelationshipBond bond = relManager.GetOrCreateBond(character, partner);
                bond.BondLevel = 10; // Anında Level 10 bağ

                Console.WriteLine($"BAŞARILI! {character.Name} ile {partner.Name} arasında efsanevi bir kıvılcım çaktı. Anında ruh eşi oldular (Bağ 10).");
            }
            else
            {
                // Başarısız: Reddedilme stresi
                character.Stress += 10;
                Console.WriteLine($"BAŞARISIZ! Zar tutmadı. {character.Name} kaba bir reddediliş yaşadı. Stres +10 arttı.");
            }
        }

        // 3. İçki Yarışması (Weekly Event)
        public static void TriggerDrinkingContest(Character character, DailyLedger ledger)
        {
            Console.WriteLine($"{character.Name} İçki Yarışmasına katılıyor!");
            int endurance = character.Attributes.ContainsKey(AttributeType.Endurance) ? character.Attributes[AttributeType.Endurance] : 10;

            OnDrinkingContestStarted?.Invoke(endurance, (isSuccess) =>
            {
                if (isSuccess)
                {
                    ledger.AddBalance(500);
                    Console.WriteLine($"{character.Name} İçki Yarışmasını Kazandı! Ödül: 500 Altın.");
                }
                else
                {
                    character.Stress += 20;
                    Console.WriteLine($"{character.Name} yarışmayı kaybetti ve masanın altına sızdı. Stres +20 arttı.");
                }
            });
        }

        // 4. İkinci Pub Savaşı (Reflex Mini Game)
        public static void TriggerPubBrawl(Character character, DailyLedger ledger)
        {
            Console.WriteLine($"Büyük bir pub kavgası çıktı! {character.Name} kavgaya karıştı.");

            int agility = character.Attributes.ContainsKey(AttributeType.Agility) ? character.Attributes[AttributeType.Agility] : 10;
            int strength = character.Attributes.ContainsKey(AttributeType.Strength) ? character.Attributes[AttributeType.Strength] : 10;

            PubBrawlConfig config = new PubBrawlConfig(agility, strength);

            OnPubBrawlTriggered?.Invoke(config, (result) =>
            {
                switch (result)
                {
                    case BrawlResult.Victory:
                        character.Stress -= 50;
                        if (!character.Traits.Contains(TraitType.Brawler))
                        {
                            character.Traits.Add(TraitType.Brawler);
                            if (character.Attributes.ContainsKey(AttributeType.Strength)) character.Attributes[AttributeType.Strength] += 5;
                            else character.Attributes[AttributeType.Strength] = 5;
                        }
                        Console.WriteLine($"{character.Name} pub'daki herkesi dövdü! Kahramanlık hissi stresi fena düşürdü (-50).");
                        break;
                    case BrawlResult.Defeat:
                        character.Stress += 50;
                        int hospitalCost = 300;
                        if (ledger.MainBalance >= hospitalCost) ledger.DeductBalance(hospitalCost);
                        else ledger.DeductBalance(ledger.MainBalance);
                        if (!character.Traits.Contains(TraitType.Bruised))
                        {
                            character.Traits.Add(TraitType.Bruised);
                        }
                        Console.WriteLine($"{character.Name} fena dayak yedi. Stres +50 ve hastane masrafları kesildi.");
                        break;
                    case BrawlResult.Fled:
                        // Prestige düşüşü loglanabilir, stress değişmez
                        if (!character.Traits.Contains(TraitType.Cowardly_Fast))
                        {
                            character.Traits.Add(TraitType.Cowardly_Fast);
                            if (character.Attributes.ContainsKey(AttributeType.Agility)) character.Attributes[AttributeType.Agility] += 5;
                            else character.Attributes[AttributeType.Agility] = 5;
                            if (character.Attributes.ContainsKey(AttributeType.Charisma)) character.Attributes[AttributeType.Charisma] -= 5;
                            else character.Attributes[AttributeType.Charisma] = -5;
                        }
                        Console.WriteLine($"{character.Name} akıllıca davrandı ve kaçtı. Stres değişmedi ama prestiji çizildi.");
                        break;
                }
            });
        }
    }
}
