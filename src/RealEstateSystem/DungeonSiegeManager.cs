using System;
using System.Collections.Generic;
using System.Linq;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;
using GameSystems.StorySystem;

namespace GameSystems.RealEstateSystem
{
    public class DungeonSiegeManager
    {
        public event Action<string>? OnLoreDiscovered;

        public void InitiateHostileTakeover(DungeonProperty targetDungeon, List<Character> cloneArmy, PoliceManager police, GlobalStoryState state)
        {
            Console.WriteLine($"[SİSTEM] {targetDungeon.Name} zindanına {cloneArmy.Count} klonluk bir orduyla işgal başlatıldı!");

            if (cloneArmy == null || cloneArmy.Count == 0)
            {
                Console.WriteLine("İşgal başarısız: Gönderilecek klon ordusu yok!");
                return;
            }

            if (cloneArmy.Any(c => !c.IsVatGrown))
            {
                Console.WriteLine("İşgal başarısız: Orduya sadece klonlar (IsVatGrown=true) katılabilir!");
                return;
            }

            // Zindan Savunma Gücü Hesaplama
            int dungeonDefensePower = (targetDungeon.BaseValue / 100) * targetDungeon.Prestige;

            // Klon Saldırı Gücü Hesaplama
            int cloneAttackPower = 0;
            foreach (var clone in cloneArmy)
            {
                int str = clone.Attributes.ContainsKey(AttributeType.Strength) ? clone.Attributes[AttributeType.Strength] : 10;
                int agi = clone.Attributes.ContainsKey(AttributeType.Agility) ? clone.Attributes[AttributeType.Agility] : 10;
                cloneAttackPower += str + agi;
            }

            Console.WriteLine($"Klon Ordusu Saldırı Gücü: {cloneAttackPower} vs Zindan Savunma Gücü: {dungeonDefensePower}");

            // Kamikaze: Klonların %80'i otomatik olarak ölüyor (destroy state)
            int casualtyCount = (int)(cloneArmy.Count * 0.8f);
            var casualties = cloneArmy.Take(casualtyCount).ToList();
            foreach (var clone in casualties)
            {
                clone.Traits.Clear();
                clone.AdvancedTraits.Clear();
                clone.Stress = 100;
                // Logically dead/destroyed for Character.cs
            }

            Console.WriteLine($"{casualtyCount} klon çatışmada can verdi...");

            if (cloneAttackPower >= dungeonDefensePower)
            {
                // BAŞARILI İŞGAL
                targetDungeon.TransferOwnership(true);
                police.CommitIllegalAction(5); // Maksimum polis baskısı / Aranma

                Console.WriteLine($"[BAŞARILI] {targetDungeon.Name} zorla ele geçirildi! Şirketin yasadışı faaliyetleri ayyuka çıktı!");

                // Büyük Uyanış - Lore Discovery
                if (!state.DiscoveredOtherUniverses)
                {
                    state.DiscoveredOtherUniverses = true;
                    string loreMessage = "Zindanın kalbine indiğinde buranın bir yeraltı mağarası değil, kızıl gökyüzü olan başka bir gezegen olduğunu fark ettin. Biz sigortacı değiliz... Biz istilacıyız!";
                    Console.WriteLine($"[LORE UNLOCKED] {loreMessage}");
                    OnLoreDiscovered?.Invoke(loreMessage);
                }
            }
            else
            {
                // BAŞARISIZ İŞGAL
                Console.WriteLine($"[BAŞARISIZ] Klon ordusu zindan savunmasını aşamadı. İşgal püskürtüldü.");
                police.CommitIllegalAction(2);
            }
        }
    }
}
