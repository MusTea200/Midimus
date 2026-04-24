using System;
using GameSystems.CoreSystem;
using GameSystems.CharacterSystem;

namespace GameSystems.StorySystem
{
    public static class BeggarStoryFactory
    {
        public static void CreateEarlyEncounter(StoryManager manager, DailyLedger ledger)
        {
            GlobalStoryState state = manager.GlobalState;

            StoryNode earlyEncounter = new StoryNode(
                id: "Beggar_Early_Encounter",
                title: "Dilenci",
                description: "Kapıda üstü başı yırtık, yüzü gölgeler içinde bir dilenci belirir. Sesi titriyordur: 'İşler iyi gidiyor gibi görünüyor patron... Sadece 5 liraya ihtiyacım var.'"
            );

            earlyEncounter.AddChoice(new StoryChoice(
                text: "Defol git buradan.",
                onSelected: () =>
                {
                    Console.WriteLine("Dilencinin titremesi durur. Çarpık bir gülümsemeyle 'Bu dünyada ezen sen olmazsan... ezilen sen olursun. Anladım.' der ve gider.");
                    state.AddFlag("Beggar_Refused");
                }
            ));

            earlyEncounter.AddChoice(new StoryChoice(
                text: "Tam 5 Lira Ver.",
                onSelected: () =>
                {
                    if (ledger.MainBalance >= 5)
                    {
                        ledger.DeductBalance(5);
                        Console.WriteLine("Yüzünde garip bir gülümseme belirir. Sesi kalınlaşır: 'İlginç bir seçim... Seni izlemeye devam edeceğim.'");
                        state.AddFlag("Beggar_Helped");
                    }
                    else
                    {
                        Console.WriteLine("Yeterli paran yok. Dilenci homurdanarak gidiyor.");
                        state.AddFlag("Beggar_Refused"); // Treats as refused if broke
                    }
                }
            ));

            earlyEncounter.AddChoice(new StoryChoice(
                text: "50 Lira Ver (Yardımsever).",
                onSelected: () =>
                {
                    if (ledger.MainBalance >= 50)
                    {
                        ledger.DeductBalance(50);
                        Console.WriteLine("Gözleri parlar: 'Sen bu oyunu kuralına göre oynamıyorsun. Bunu... unutmayacağım.'");
                        state.AddFlag("Beggar_Invested");
                    }
                    else
                    {
                        Console.WriteLine("Yeterli paran yok. Dilenci homurdanarak gidiyor.");
                        state.AddFlag("Beggar_Refused"); // Treats as refused if broke
                    }
                }
            ));

            manager.RegisterNode(earlyEncounter);
        }

        public static void CreateLateEncounter(StoryManager manager, DailyLedger ledger, Character character)
        {
            GlobalStoryState state = manager.GlobalState;

            StoryNode lateEncounter = new StoryNode(
                id: "Beggar_Late_Encounter",
                title: "Dungeon Lord'un Yüzleşmesi",
                description: "Şehrin en değerli zindanını satın almak için ofise girdin. Koltuktaki şık takımlı Lord sana döner... O dilenci! Yüzünde ürkütücü bir gülümseme var."
            );

            lateEncounter.AddChoice(new StoryChoice(
                text: "Yüzleşmeye Başla",
                onSelected: () =>
                {
                    if (state.HasFlag("Beggar_Refused"))
                    {
                        Console.WriteLine("Dungeon Lord: 'Bana 5 lira bile vermemiştin. Kararlarının bir önemi olmadığını sanıyordun değil mi? Şimdi iflas edeceksin!'");
                        ledger.DeductBalance(5000);
                        character.Stress += 100;
                        Console.WriteLine("Kasadan 5000 Altın kesildi ve Karakterin stresi tavan yaptı!");
                    }
                    else if (state.HasFlag("Beggar_Helped"))
                    {
                        Console.WriteLine("Dungeon Lord: 'O gün o 5 lirayı vermeseydin, şu an bu binayı başına yıkıyor olurdum. Şanslısın.'");
                        ledger.AddBalance(2000);
                        Console.WriteLine("Zindan alımı için sana 2000 Altın indirim/para iadesi yapıldı.");
                    }
                    else if (state.HasFlag("Beggar_Invested"))
                    {
                        Console.WriteLine("Dungeon Lord: 'Sen bana inandın. Tüm borçlarını ödedim. Biz seninle çok eğleneceğiz patron.'");
                        ledger.AddBalance(10000);
                        // Prestij statüsü eklenecekse buraya eklenebilir. Şimdilik logluyoruz.
                        Console.WriteLine("Kasanıza 10000 Altın devasa servet eklendi ve Prestij tavan yaptı!");
                    }
                    else
                    {
                        Console.WriteLine("Dungeon Lord seni tanımadı... (Herhangi bir flag bulunamadı)");
                    }
                }
            ));

            manager.RegisterNode(lateEncounter);
        }
    }
}
