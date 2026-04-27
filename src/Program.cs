using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;
using GameSystems.StorySystem;
using GameSystems.CitySystem;
using GameSystems.RealEstateSystem;

namespace GameSystems
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- DEPENDENCY INJECTION & SETUP ---
            SimulationManager simManager = new SimulationManager();
            DailyLedger ledger = new DailyLedger(500); // Start with some gold for testing
            GlobalStoryState storyState = new GlobalStoryState();
            List<Character> roster = new List<Character>();

            // Create the main character
            Character mainCharacter = new Character("Jules");
            roster.Add(mainCharacter);

            PoliceManager police = new PoliceManager();
            PrologueManager prologue = new PrologueManager(simManager, ledger, storyState, roster);
            TempleFacility lab = new TempleFacility(ledger); // The Mad Science Lab is inside TempleFacility
            UndergroundArenaFacility arena = new UndergroundArenaFacility(ledger, police);

            // Subscribe to Events
            prologue.OnPlayerKilledInTraffic += () =>
            {
                Console.WriteLine("\n*** SİSTEM UYARISI: [DÖRDÜNCÜ DUVAR] TIR ÇARPTI! OYUN RESETLENİYOR... ***\n");
                Environment.Exit(0);
            };

            arena.OnPoliceRaid += () =>
            {
                Console.WriteLine("\n*** SİSTEM UYARISI: [POLİS BASKINI] Mekan mühürlendi! ***\n");
            };

            // Setup Sanity System
            SanityDistortionManager sanity = new SanityDistortionManager(simManager);
            sanity.OnScreenGlitchTriggered += () => Console.WriteLine("\n*** SİSTEM UYARISI: [GLITCH] EKRAN DALGALANIYOR! ***\n");
            sanity.OnCreepyNPCBehavior += (c) => Console.WriteLine($"\n*** SİSTEM UYARISI: {c.Name} sana boş gözlerle bakıyor... ***\n");

            // Setup Meta Horror System
            MetaHorrorManager metaHorror = new MetaHorrorManager(simManager);
            metaHorror.OnFakeCrashToDesktop += () => Console.WriteLine("\n*** FATAL ERROR 0x00000000. SAHTE MASAÜSTÜNE GEÇİLİYOR... ***\n");
            metaHorror.OnFlashFakeTerminal += () => Console.WriteLine("\n*** [CMD.EXE FLASH] C:\\> del /s /q System32 ***\n");

            // --- GAME LOOP ---
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n================================================");
                Console.WriteLine("    ZİNDAN SİGORTA A.Ş. - YÖNETİCİ TERMİNALİ");
                Console.WriteLine("================================================");
                Console.WriteLine($"Gün: {simManager.CurrentDay} | Kasa: {ledger.MainBalance} Altın | Karma: {simManager.HiddenKarmaScore}");
                Console.WriteLine($"Laboratuvar Seviyesi: {lab.LabLevel} | Aktif Klonlar: {lab.ActiveVats.Count} | Hazır Bedenler: {lab.ReadyBodies.Count}");
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("[1] Oyuna Başla (Prologue - Sokaktaki Ceset Seçimi)");
                Console.WriteLine("[2] Laboratuvarda Klon Üret (Maliyet: 1000/2000 Altın)");
                Console.WriteLine("[3] Yeraltı Arenasına Ana Karakteri Gönder (Savaş)");
                Console.WriteLine("[4] Günü Bitir (Maliyetleri hesapla, Klonları büyüt)");
                Console.WriteLine("[0] Çıkış");
                Console.Write("Seçiminiz: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\n>>> PROLOGUE BAŞLIYOR <<<");
                        prologue.TriggerTimeCapsuleEvent(mainCharacter);

                        Console.WriteLine("\nSokakta kanlar içinde bir beden gördün. Ne yapacaksın?");
                        Console.WriteLine("  A) Cesedi Ara (Para bul)");
                        Console.WriteLine("  B) Ambulans Çağır (İyilik yap)");
                        Console.WriteLine("  C) Görmezden Gel (Geç git)");
                        Console.Write("Seçim: ");
                        string? pChoice = Console.ReadLine()?.ToUpper();

                        if (pChoice == "A") prologue.TriggerAlleyEncounter(EncounterChoice.CheckBody);
                        else if (pChoice == "B") prologue.TriggerAlleyEncounter(EncounterChoice.CallAmbulance);
                        else if (pChoice == "C") prologue.TriggerAlleyEncounter(EncounterChoice.Ignore);
                        else Console.WriteLine("Geçersiz seçim. Ceset toz oldu.");
                        break;

                    case "2":
                        Console.WriteLine("\n>>> LABORATUVAR: KLON ÜRETİMİ <<<");
                        Console.WriteLine("Risk Seçimi: [1] Düşük Risk (1000 Altın, 3 Gün) | [2] Yüksek Risk (2000 Altın, 7 Gün)");
                        Console.Write("Seçim: ");
                        string? riskChoice = Console.ReadLine();
                        bool isHighRisk = (riskChoice == "2");
                        lab.StartGrowingBody(isHighRisk);
                        break;

                    case "3":
                        Console.WriteLine("\n>>> YERALTI ARENASI <<<");
                        Console.Write("Bahis Miktarı Girin: ");
                        if (int.TryParse(Console.ReadLine(), out int bet))
                        {
                            arena.EnterTournament(mainCharacter, ArenaLeague.MonsterLeague, bet); // Monster league mock, since we don't have cyborg set up here natively
                        }
                        else
                        {
                            Console.WriteLine("Hatalı bahis miktarı.");
                        }
                        break;

                    case "4":
                        Console.WriteLine("\n>>> GÜN BİTİYOR <<<");

                        // Klonların gününü geçir
                        foreach(var vat in lab.ActiveVats.ToArray())
                        {
                            vat.DecrementDay();
                            if (vat.IsReady)
                            {
                                lab.ActiveVats.Remove(vat);
                                lab.ReadyBodies.Add(vat);
                            }
                        }

                        // Simulation Manager hooks
                        // Note: Empty lists used as placeholders to simply trigger end of day hooks
                        simManager.RunDailyExpeditions(new List<GameSystems.QuestSystem.InsurancePolicy>(), roster, storyState, ledger, mainCharacter, lab);

                        // Karma distorsiyonlarını kontrol et
                        sanity.CheckForDistortions();
                        metaHorror.TickHorrorEvents();

                        Console.WriteLine("Gün başarıyla atlandı.");
                        break;

                    case "0":
                        isRunning = false;
                        Console.WriteLine("Sistem kapatılıyor...");
                        break;

                    default:
                        Console.WriteLine("Geçersiz komut.");
                        break;
                }
            }
        }
    }
}
