using System;
using GameSystems.CoreSystem;
using GameSystems.CharacterSystem;

namespace GameSystems.StorySystem
{
    public static class RehabStoryFactory
    {
        private static Random _rng = new Random();

        // 1. SPA Events (Sonsuz Buhar Odası & Masör Mimic)
        public static void TriggerSpaEvents(StoryManager manager, Character character, DailyLedger ledger, PlayerManager player)
        {
            double roll = _rng.NextDouble();

            // %3 ihtimalle Sonsuz Buhar Odası
            if (roll <= 0.03)
            {
                Console.WriteLine("Sonsuz Buhar Odası: Zaman burada farklı akıyor gibi...");
                character.Stress = 0;

                if (!character.Traits.Contains(TraitType.Deja_Vu))
                {
                    character.Traits.Add(TraitType.Deja_Vu);
                    Console.WriteLine($"{character.Name} 'Deja_Vu' (Tuzaklardan %15 kaçınma şansı) özelliğini kazandı!");
                }
            }
            // %2 ihtimalle Masör Mimic (0.03 ile 0.05 arası)
            else if (roll <= 0.05)
            {
                Console.WriteLine("Masör Mimic: Çamur banyosu canlandı!");

                int strength = character.Attributes.ContainsKey(AttributeType.Strength) ? character.Attributes[AttributeType.Strength] : 10;
                int checkRoll = _rng.Next(1, 101);

                if (checkRoll <= strength)
                {
                    player.AddMaterial(CraftingMaterial.Slime_Core, 1);
                    Console.WriteLine($"{character.Name} canavarı alt etti ve nadir materyal 'Slime_Core' kazandı!");
                }
                else
                {
                    character.Stress += 50;
                    if (!character.Traits.Contains(TraitType.Trust_Issues))
                    {
                        character.Traits.Add(TraitType.Trust_Issues);
                        Console.WriteLine($"{character.Name} çok korktu ve kalıcı 'Trust_Issues' (Güven Sorunu) özelliğini kazandı.");
                    }
                    else
                    {
                        Console.WriteLine($"{character.Name} fena halde korktu. Stres arttı (+50).");
                    }
                }
            }
        }

        // 2. Pub Events (Gölge Komisyoncu)
        public static void TriggerPubEvents(StoryManager manager, Character character, DailyLedger ledger)
        {
            double roll = _rng.NextDouble();

            // %5 ihtimalle Gölge Komisyoncu
            if (roll <= 0.05)
            {
                Console.WriteLine("Gölge Komisyoncu: Otomatik kazanç vadeden bir yatırım fırsatı.");

                // Seçenekleri simüle ediyoruz. (Event trigger tarzı)
                StoryNode node = new StoryNode(
                    id: "Shadow_Broker_Investment",
                    title: "Gölge Komisyoncu",
                    description: "Barda karanlık bir tip sana yaklaştı. 'Sana günlük kazanç sağlayacak bir sistemim var. Sadece 500 altın.' diyor."
                );

                node.AddChoice(new StoryChoice(
                    text: "Yatırım Yap (-500 Altın)",
                    onSelected: () =>
                    {
                        if (ledger.MainBalance >= 500)
                        {
                            ledger.DeductBalance(500);
                            manager.GlobalState.AddFlag("Active_Goblin_Investment");
                            Console.WriteLine("Yatırım yapıldı. GlobalStoryState'e 'Active_Goblin_Investment' eklendi.");
                        }
                        else
                        {
                            Console.WriteLine("Yeterli altın yok.");
                        }
                    }
                ));

                node.AddChoice(new StoryChoice(
                    text: "İlgilenmiyorum.",
                    onSelected: () => Console.WriteLine("Teklifi reddettin.")
                ));

                manager.RegisterNode(node);
                manager.TriggerNode("Shadow_Broker_Investment");
            }
        }

        // 3. Ice Cream Shop Events (Mana Naneli Dondurma)
        public static void TriggerIceCreamEvents(StoryManager manager, Character character)
        {
            double roll = _rng.NextDouble();

            // %4 ihtimalle Mana Naneli Dondurma
            if (roll <= 0.04)
            {
                Console.WriteLine("Mana Naneli Dondurma: Dondurma beynini donduruyor gibi hissediyorsun!");

                int endurance = character.Attributes.ContainsKey(AttributeType.Endurance) ? character.Attributes[AttributeType.Endurance] : 10;
                int checkRoll = _rng.Next(1, 101);

                if (checkRoll <= endurance)
                {
                    if (character.Attributes.ContainsKey(AttributeType.Intelligence))
                    {
                        character.Attributes[AttributeType.Intelligence] += 5;
                    }
                    else
                    {
                        character.Attributes[AttributeType.Intelligence] = 5;
                    }
                    Console.WriteLine($"Başarılı! Zihin açıldı. {character.Name} kalıcı olarak Intelligence +5 kazandı.");
                }
                else
                {
                    character.Stress -= 40;
                    manager.GlobalState.AddFlag($"Brain_Freeze_Debuff_{character.Id}");
                    Console.WriteLine($"Başarısız! Beyin donması yaşandı. Stres düştü (-40) ancak günlük 'Brain_Freeze_Debuff' (Zeka -%50) alındı.");
                }
            }
        }
    }
}
