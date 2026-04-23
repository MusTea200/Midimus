using System;
using System.Collections.Generic;
using GameSystems.CoreSystem;
using GameSystems.CharacterSystem;

namespace GameSystems.StorySystem
{
    public static class CreatureStoryFactory
    {
        // Örnek senaryoyu manager'a yükler
        public static void BuildCreatureStory(StoryManager manager, DailyLedger ledger, Character involvedCharacter)
        {
            GlobalStoryState state = manager.GlobalState;

            // --- DÜĞÜM 1: İlk Karşılaşma ---
            StoryNode node1 = new StoryNode(
                id: "Creature_Encounter_01",
                title: "Zindandan Gelen Sürpriz",
                description: $"{involvedCharacter.Name} zindandan dönerken çantasında sevimli ama tuhaf bir yavru yaratık buldu."
            );

            node1.AddChoice(new StoryChoice(
                text: "Sahiplen ve Besle",
                onSelected: () =>
                {
                    state.AddFlag("Creature_Adopted");
                    Console.WriteLine("Yavruyu sahiplendin. Artık şirketin bir maskotu var!");
                }
            ));

            node1.AddChoice(new StoryChoice(
                text: "Kolyeyle Doğaya Sal",
                onSelected: () =>
                {
                    state.AddFlag("Creature_Released_With_Necklace");
                    Console.WriteLine("Yavrunun boynuna bir takip kolyesi takip doğaya saldın.");
                }
            ));

            node1.AddChoice(new StoryChoice(
                text: "Yavruyu İtlaf Et (Risk Alma)",
                onSelected: () =>
                {
                    state.AddFlag("Mother_Enraged");
                    involvedCharacter.Stress += 30; // Acımasız karar strese sebep olur
                    Console.WriteLine("Yavruyu yok ettin. Kahramanın vicdan azabıyla strese girdi...");
                }
            ));

            manager.RegisterNode(node1);


            // --- DÜĞÜM 2: Sonuçlar (Kelebek Etkisi) ---
            StoryNode node2 = new StoryNode(
                id: "Creature_Resolution_02",
                title: "Yavru Yaratığın Akıbeti",
                description: "Geçmişte verdiğin karar bugün meyvesini veriyor..."
            );

            // Sadece Creature_Adopted bayrağı olanlar bu seçeneği görebilir
            node2.AddChoice(new StoryChoice(
                text: "Maskot büyüdü ve kahramana dönüştü!",
                requiredFlags: new List<string> { "Creature_Adopted" },
                onSelected: () =>
                {
                    Console.WriteLine("Yaratık evcilleşti ve şirketin en sadık savaşçısı oldu!");
                    // (Burada listeye yeni bir Character objesi eklenebilir)
                }
            ));

            // Sadece Creature_Released_With_Necklace bayrağı olanlar bu seçeneği görebilir
            node2.AddChoice(new StoryChoice(
                text: "Zindandaki dost...",
                requiredFlags: new List<string> { "Creature_Released_With_Necklace" },
                onSelected: () =>
                {
                    Console.WriteLine("Vaktiyle saldığın yaratık, zindanda sıkışan kahramanlarına gizlice yardım etmeye başladı.");
                }
            ));

            // Sadece Mother_Enraged bayrağı olanlar bu seçeneği görebilir
            node2.AddChoice(new StoryChoice(
                text: "ANNE YARATIK BASKINI!",
                requiredFlags: new List<string> { "Mother_Enraged" },
                onSelected: () =>
                {
                    Console.WriteLine("Yavrusunun intikamını almak isteyen devasa bir anne yaratık ofisi bastı!");
                    ledger.DeductBalance(5000); // 5000 altın tazminat/hasar cezası
                    Console.WriteLine("Büyük hasar alındı: 5000 Altın kaybedildi.");
                }
            ));

            manager.RegisterNode(node2);
        }
    }
}
