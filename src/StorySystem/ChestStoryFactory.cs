using System;
using System.Collections.Generic;
using GameSystems.CoreSystem;
using GameSystems.CharacterSystem;

namespace GameSystems.StorySystem
{
    public static class ChestStoryFactory
    {
        public static void BuildChestStory(StoryManager manager, DailyLedger ledger, Character currentCharacter)
        {
            GlobalStoryState state = manager.GlobalState;
            Random rng = new Random();

            // --- DÜĞÜM 1: Zindan Öncesi Karar ---
            StoryNode node1 = new StoryNode(
                id: "Chest_Start_01",
                title: "Zindan Öncesi Karar",
                description: $"{currentCharacter.Name} sana geldi ve girmek istediği zorlu zindan için tavsiye ve destek istiyor. Ne yapacaksın?"
            );

            node1.AddChoice(new StoryChoice(
                text: "Karakteri doyur, tavsiye ver ve destekle.",
                onSelected: () =>
                {
                    ledger.DeductBalance(50); // Az para harca
                    Console.WriteLine("Karakter zindandan başarıyla döndü.");
                    manager.TriggerNode("Chest_Mystery_02"); // Node 2'ye git
                }
            ));

            node1.AddChoice(new StoryChoice(
                text: "Sadece taktik ver, para harcama.",
                onSelected: () =>
                {
                    Console.WriteLine("Para harcamadın ama karakter hırpalanmış döndü.");
                    manager.TriggerNode("Chest_Battered_04"); // Node 4'e git
                }
            ));

            node1.AddChoice(new StoryChoice(
                text: "Görevi reddet.",
                onSelected: () =>
                {
                    Console.WriteLine("Görevi reddettin. Hikaye bitti.");
                }
            ));

            manager.RegisterNode(node1);


            // --- DÜĞÜM 2: Gizemli Sandık ve İrade Sınavı ---
            StoryNode node2 = new StoryNode(
                id: "Chest_Mystery_02",
                title: "Gizemli Sandık ve İrade Sınavı",
                description: $"{currentCharacter.Name} zindanı kazandı ve elinde kilitli, gizemli bir sandıkla döndü. Sandığı masanın üzerine koyup yorgunlukla gülümsedi. Gözün sandıkta kaldı. Ne yapacaksın?"
            );

            node2.AddChoice(new StoryChoice(
                text: "Gece olunca sandığı gizlice çal.",
                onSelected: () =>
                {
                    Console.WriteLine("Gece sandığı çaldın. Sabah karakter sandığı bulamayınca şüpheyle sana sordu.");
                    manager.TriggerNode("Chest_Confrontation_03"); // Node 3'e git
                }
            ));

            node2.AddChoice(new StoryChoice(
                text: "Karakteri tebrik et ve sandığa dokunma (Ahlaklı Yol).",
                onSelected: () =>
                {
                    // Dinamik Matematiksel Formül: Chance = 50 - (Stress / 2)
                    int chance = 50 - (currentCharacter.Stress / 2);
                    int roll = rng.Next(0, 101);

                    if (roll <= chance)
                    {
                        state.AddFlag("Gifted_Chest_Initiative");
                        Console.WriteLine($"{currentCharacter.Name}: 'Senin desteğin olmasaydı başaramazdım' dedi ve sandığı sana hediye etti!");
                    }
                    else
                    {
                        state.AddFlag("Missed_Chest");
                        Console.WriteLine($"{currentCharacter.Name} teşekkür etti, sandığını sırtladı ve odasına çekildi.");
                    }
                    Console.WriteLine("Hikaye bitti.");
                }
            ));

            manager.RegisterNode(node2);


            // --- DÜĞÜM 3: Çalıntı Sandık Yüzleşmesi ---
            StoryNode node3 = new StoryNode(
                id: "Chest_Confrontation_03",
                title: "Çalıntı Sandık Yüzleşmesi",
                description: "Sabah oldu. Karakter sandığını arıyor ve şüpheyle gözlerinin içine bakıyor. Ne diyeceksin?"
            );

            node3.AddChoice(new StoryChoice(
                text: "Beceriksizce Yalan Söyle.",
                onSelected: () =>
                {
                    currentCharacter.Stress += 100;
                    state.AddFlag("Chest_Story_Ended_Badly");
                    Console.WriteLine("Yakalandın! Karakter sandığı geri aldı ve sana güveni bitti. Hikaye bitti.");
                }
            ));

            node3.AddChoice(new StoryChoice(
                text: "İnandırıcı Yalan Söyle.",
                onSelected: () =>
                {
                    state.AddFlag("Stolen_Chest_Secret");
                    Console.WriteLine("Yakalanmadın. Şaşkın şaşkın etrafı arayan bir karakter var. İlerde sandığı gizlice açman gerekecek, yoksa ayıkabilir. Hikaye bitti.");
                }
            ));

            node3.AddChoice(new StoryChoice(
                text: "Doğruyu söyle ve itiraf et.",
                onSelected: () =>
                {
                    state.AddFlag("Gifted_Chest_Honest");
                    Console.WriteLine("İtiraf ettin. Karakter bu erdemli davranışından dolayı seni affetti ve sandığı hediye etti. Hikaye bitti.");
                }
            ));

            manager.RegisterNode(node3);


            // --- DÜĞÜM 4: Hırpalanmış Karakterle Yüzleşme ---
            StoryNode node4 = new StoryNode(
                id: "Chest_Battered_04",
                title: "Hırpalanmış Karakterle Yüzleşme",
                description: "Destek vermediğin karakter hırpalanmış şekilde döndü. Tazminat ödüyorsun. Şimdi ona karşı nasıl bir tavır almalısın?"
            );

            node4.AddChoice(new StoryChoice(
                text: "Hırpalanmış olması bizi ilgilendirmez, biz sadece işimizi yapıyoruz.",
                onSelected: () =>
                {
                    currentCharacter.Stress += 30;
                    Console.WriteLine("Soğuk davrandın. Karakter streslendi. Hikaye bitti.");
                }
            ));

            node4.AddChoice(new StoryChoice(
                text: "Biraz dinlenmesi gerektiğini söyle.",
                onSelected: () =>
                {
                    Console.WriteLine("Empatik yaklaştın. Karakterin stresi etkilenmedi. Hikaye bitti.");
                }
            ));

            manager.RegisterNode(node4);
        }
    }
}
