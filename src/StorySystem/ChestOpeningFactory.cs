using System;
using System.Collections.Generic;
using GameSystems.CoreSystem;
using GameSystems.CharacterSystem;

namespace GameSystems.StorySystem
{
    public static class ChestOpeningFactory
    {
        public static void BuildChestOpeningStory(
            StoryManager manager,
            DailyLedger ledger,
            PlayerManager player,
            GamePhaseManager phaseManager,
            Character currentCharacter,
            List<Character> allActiveCharacters)
        {
            if (!phaseManager.IsCityPhaseUnlocked)
            {
                // Sandık senaryosu sadece Şehir Fazı açıkken tetiklenebilir
                return;
            }

            GlobalStoryState state = manager.GlobalState;
            Random rng = new Random();

            // SENARYO A: Hediye Edilen Sandık
            if (state.HasFlag("Gifted_Chest_Honest") || state.HasFlag("Gifted_Chest_Initiative"))
            {
                StoryNode giftedNode = new StoryNode(
                    id: "Chest_Opening_Gifted",
                    title: "Demirci Kitabı Keşfi",
                    description: $"{currentCharacter.Name} zindandan dönerken elinde saf bakırdan, parlak bir anahtar tutuyor. 'Bu anahtar o sandığa ait olabilir mi?' diyerek sana uzatıyor. Anahtarı deliğe sokuyorsun ve 'Tık!'... Sandık açılıyor."
                );

                giftedNode.AddChoice(new StoryChoice(
                    text: "İçine bak...",
                    onSelected: () =>
                    {
                        Console.WriteLine("İçinden 'Demirci Frank'in El Kitabı, Yazar Frank Frankson' yazan bir kitap çıkıyor. Kapağı açtığın an sayfalar hızla akıyor. Gözlerinin önünde örse vuran çekiçler, kıvılcımlar ve harlanan ateşin vizyonu beliriyor. Son sayfa kapanıyor... Artık demircilik hakkında her şeyi biliyorsun.");

                        state.AddFlag("Blacksmith_Unlocked");
                        state.AddFlag("Crafting_Discount");
                        state.AddFlag("Enchanting_Knowledge");
                        state.AddFlag("Excavator_Blueprint");

                        Console.WriteLine("Yeni bilgiler kazanıldı. Hikaye Bitti.");
                    }
                ));

                manager.RegisterNode(giftedNode);
            }

            // SENARYO B: Çalıntı Sandık
            if (state.HasFlag("Stolen_Chest_Secret"))
            {
                int attemptsRemaining = 3;

                // Recursive/Döngüsel kullanım için action tanımlaması
                Action? setupLockpickNode = null;

                setupLockpickNode = () =>
                {
                    StoryNode stolenNode = new StoryNode(
                        id: $"Chest_Opening_Stolen_{attemptsRemaining}",
                        title: "Çalıntı Sandık",
                        description: $"Gece yarısı. Sandık masanda duruyor. Kilidi kırmak için {attemptsRemaining} hakkın var. Maymuncuğu deliğe sokuyorsun..."
                    );

                    stolenNode.AddChoice(new StoryChoice(
                        text: "Kilidi Kurcala (El Becerisi Kullan)",
                        onSelected: () =>
                        {
                            if (player.RollSleightOfHand())
                            {
                                // Durum 1: Başarılı
                                Console.WriteLine("Tık! Kilit açıldı.");
                                Console.WriteLine("İçinden 'Demirci Frank'in El Kitabı, Yazar Frank Frankson' yazan bir kitap çıkıyor. Kapağı açtığın an sayfalar hızla akıyor. Gözlerinin önünde örse vuran çekiçler, kıvılcımlar ve harlanan ateşin vizyonu beliriyor. Son sayfa kapanıyor... Artık demircilik hakkında her şeyi biliyorsun.");

                                state.AddFlag("Blacksmith_Unlocked");
                                state.AddFlag("Crafting_Discount");
                                state.AddFlag("Enchanting_Knowledge");
                                state.AddFlag("Excavator_Blueprint");

                                Console.WriteLine("Yeni bilgiler kazanıldı. Hikaye Bitti.");
                            }
                            else
                            {
                                // Durum 2: Başarısızlık ve İkna zarı
                                Console.WriteLine("Şangırtı! Maymuncuk kaydı ve ses çıkardı. Karakter uyanıp odaya geldi.");

                                if (player.RollPersuasion())
                                {
                                    attemptsRemaining--;
                                    Console.WriteLine("İkna yeteneğinle durumu kurtardın. Karakter geri uyudu.");

                                    if (attemptsRemaining > 0)
                                    {
                                        // Tekrar dene
                                        setupLockpickNode?.Invoke();
                                        manager.TriggerNode($"Chest_Opening_Stolen_{attemptsRemaining}");
                                    }
                                    else
                                    {
                                        TriggerExplosionPenalty(state, ledger, allActiveCharacters, rng);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("İkna edemedin!");
                                    TriggerExplosionPenalty(state, ledger, allActiveCharacters, rng);
                                }
                            }
                        }
                    ));

                    manager.RegisterNode(stolenNode);
                };

                // İlk düğümü kur
                setupLockpickNode?.Invoke();
            }
        }

        // Durum 3: Patlama Durumu
        private static void TriggerExplosionPenalty(GlobalStoryState state, DailyLedger ledger, List<Character> allActiveCharacters, Random rng)
        {
            Console.WriteLine("Sandık aniden alev alıyor! İçindeki büyü mekanizması kendini imha ediyor ve ofiste yangın çıkıyor!");

            // Restorasyon parası kes
            ledger.DeductBalance(15000);
            Console.WriteLine("Ofis restorasyonu için kasadan 15000 Altın kesildi.");

            // Tüm karakterlere +40 Stres ve %30 ihtimalle Pyrophobia
            foreach (var character in allActiveCharacters)
            {
                character.Stress += 40;

                if (rng.NextDouble() <= 0.30)
                {
                    if (!character.Phobias.Contains(PhobiaType.Pyrophobia))
                    {
                        character.Phobias.Add(PhobiaType.Pyrophobia);
                        Console.WriteLine($"{character.Name} artık ateşten korkuyor (Pyrophobia).");
                    }
                }
            }

            state.AddFlag("Chest_Destroyed_Fire");
            Console.WriteLine("Hikaye Felaketle Bitti.");
        }
    }
}
