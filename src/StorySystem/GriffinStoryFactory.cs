using System;
using System.Collections.Generic;
using GameSystems.CoreSystem;
using GameSystems.CharacterSystem;

namespace GameSystems.StorySystem
{
    public static class GriffinStoryFactory
    {
        public static void CreateEggDiscoveryNode(StoryManager manager, DailyLedger ledger)
        {
            GlobalStoryState state = manager.GlobalState;

            StoryNode node = new StoryNode(
                id: "Griffin_Egg_Discovery",
                title: "Gizemli Yumurta",
                description: "Sabah ofis kapısında sepet içinde devasa, sıcak bir yumurta buldun. Üzerinde garip desenler var."
            );

            node.AddChoice(new StoryChoice(
                text: "Kuluçka makinesi al ve yumurtaya bak.",
                onSelected: () =>
                {
                    if (ledger.MainBalance >= 200)
                    {
                        ledger.DeductBalance(200);
                        state.AddFlag("Egg_Incubating");
                        Console.WriteLine("Kuluçka makinesi alındı ve yumurta güvene alındı.");
                    }
                    else
                    {
                        Console.WriteLine("Yeterli altın yok (-200 Altın gerekiyor). Yumurtayı köşeye bıraktın ama kırıldı.");
                        // Alternatively, we could fail gracefully.
                    }
                }
            ));

            node.AddChoice(new StoryChoice(
                text: "Çöpe at.",
                onSelected: () =>
                {
                    Console.WriteLine("Yumurtayı çöpe attın. İçinden ne çıkacağını asla bilemeyeceksin.");
                    // Hikaye biter
                }
            ));

            manager.RegisterNode(node);
        }

        public static void CreateWarningLetterNode(StoryManager manager, List<Character> activeCharacters)
        {
            GlobalStoryState state = manager.GlobalState;

            StoryNode node = new StoryNode(
                id: "Griffin_Warning_Letter",
                title: "İsimsiz Mektup",
                description: "Postadan isimsiz bir mektup çıktı: 'O kuluçkaya yatırdığın şey bir Griffin yumurtası! Çıkarsa ofisi kan gölüne çevirir. Hemen imha et!'"
            );

            node.AddChoice(new StoryChoice(
                text: "Mektuba inan ve yumurtayı çekiçle kır.",
                onSelected: () =>
                {
                    Console.WriteLine("Yumurtayı kırdığın an odaya bir karakterin giriyor ve dehşet içinde sana bakıyor. İçinden sadece sarı bir sıvı akıyor. Oyun Sistemi: 'İsimsiz bir kağıt parçası yüzünden masum bir hayatı bitirdin. Her denilene böyle kolay mı kanarsın?'");

                    foreach (var character in activeCharacters)
                    {
                        character.Stress += 50;
                    }

                    state.AddFlag("Egg_Destroyed_Gullible");
                },
                requiredFlags: new List<string> { "Egg_Incubating" }
            ));

            node.AddChoice(new StoryChoice(
                text: "Mektubu yırtıp at. Kuluçkaya devam et.",
                onSelected: () =>
                {
                    Console.WriteLine("Mektubu yırttın. İnandığın yoldan dönmeyeceksin.");
                    state.AddFlag("Egg_Kept_Defiant");
                },
                requiredFlags: new List<string> { "Egg_Incubating" }
            ));

            manager.RegisterNode(node);
        }

        public static void CreateHatchingNode(StoryManager manager, DailyLedger ledger, List<Character> activeCharacters, Character targetCharacter)
        {
            GlobalStoryState state = manager.GlobalState;
            Random rng = new Random();

            StoryNode node = new StoryNode(
                id: "Griffin_Hatching",
                title: "Kuluçka Sonucu",
                description: "Çat... Çatırtı! Kuluçka makinesinin camı kırıldı. İçinden gerçekten de bir Griffin yavrusu çıktı!"
            );

            node.AddChoice(new StoryChoice(
                text: "Gözlemle",
                onSelected: () =>
                {
                    double roll = rng.NextDouble();
                    if (roll <= 0.5) // %50 Felaket
                    {
                        Console.WriteLine("Yavru aniden vahşileşti! Ofisi darmadağın etti, çalışanları yaraladı ve camı kırıp kaçtı. Bazen iyi niyet sadece ahmaklıktır.");

                        int penalty = 1000;
                        if (ledger.MainBalance >= penalty)
                        {
                            ledger.DeductBalance(penalty);
                        }
                        else
                        {
                            ledger.DeductBalance(ledger.MainBalance);
                        }

                        foreach (var character in activeCharacters)
                        {
                            character.Stress += 40;
                        }

                        state.AddFlag("Griffin_Disaster");
                    }
                    else // %50 Mucize
                    {
                        Console.WriteLine("Yavru sana şefkatle yaklaştı. Hızla büyüdü ve ofisin maskotu oldu. Beklediğine değdi!");

                        Mount legendaryGriffin = new Mount("Legendary Royal Griffin", 0, 0.80f); // Bedava, %80 Stress Reduction
                        targetCharacter.ActiveMount = legendaryGriffin;

                        Console.WriteLine($"{targetCharacter.Name} efsanevi bir binek olan 'Legendary Royal Griffin' kuşandı!");

                        state.AddFlag("Griffin_Mount_Acquired");
                    }
                },
                requiredFlags: new List<string> { "Egg_Kept_Defiant" }
            ));

            manager.RegisterNode(node);
        }
    }
}
