using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.StorySystem
{
    public enum EncounterChoice
    {
        CheckBody,
        CallAmbulance,
        Ignore
    }

    public class PrologueManager
    {
        private SimulationManager _simManager;
        private DailyLedger _ledger;
        private GlobalStoryState _storyState;
        private List<Character> _roster;

        public event Action? OnPlayerKilledInTraffic;

        public PrologueManager(SimulationManager simManager, DailyLedger ledger, GlobalStoryState storyState, List<Character> roster)
        {
            _simManager = simManager;
            _ledger = ledger;
            _storyState = storyState;
            _roster = roster;
        }

        public void TriggerTimeCapsuleEvent(Character mainCharacter)
        {
            Console.WriteLine("--- BAŞLANGIÇ ---");
            Console.WriteLine("Şehrin pis sokaklarında uyandın. Cebinde hiç paran yok, başın inanılmaz ağrıyor.");

            // 0 altın
            if (_ledger.MainBalance > 0)
            {
                _ledger.DeductBalance(_ledger.MainBalance);
            }

            mainCharacter.Stress = 95;
            if (!mainCharacter.Traits.Contains(TraitType.Broken_Heart)) // Depressed benzeri bir Trait
            {
                mainCharacter.Traits.Add(TraitType.Broken_Heart);
            }

            _storyState.AddFlag("DreamCar"); // Motivasyon bayrağı

            Console.WriteLine("Tek bir hedefin var: O hayalindeki arabayı ('DreamCar') almak ve bu bataklıktan kurtulmak.");
        }

        public void TriggerAlleyEncounter(EncounterChoice choice)
        {
            Console.WriteLine("Ara sokakta yürürken kanlar içinde yerde yatan birini görüyorsun. Yanında parlak bir evrak çantası var.");

            switch (choice)
            {
                case EncounterChoice.CheckBody:
                    Console.WriteLine("Cesedin/Yaralının yanına çöktün ve çantayı karıştırdın...");
                    _ledger.AddBalance(500);
                    // UnclaimedInsurancePolicy ekleme (Şimdilik mock olarak logluyoruz, Item class'ına eklenebilir)
                    Console.WriteLine("Envanterine 'Sahipsiz Poliçe' (UnclaimedInsurancePolicy) eklendi ve 500 Altın buldun.");
                    _simManager.AddKarma(-5);
                    break;

                case EncounterChoice.CallAmbulance:
                    Console.WriteLine("Hemen ambulansı aradın ve başucunda bekledin...");
                    _simManager.AddKarma(15);

                    // Yeni Karakter (Maceracı/Elit)
                    Character rescuedHero = new Character("Yaralı Maceracı") { Gender = GenderType.Male };
                    rescuedHero.Attributes[AttributeType.Strength] = 80;
                    rescuedHero.Traits.Add(TraitType.Brave);
                    _roster.Add(rescuedHero);

                    Console.WriteLine($"{rescuedHero.Name} kurtuldu ve sana hayatını borçlu! Şirketine ücretsiz katıldı. (Bond seviyesi: Minnettar)");
                    break;

                case EncounterChoice.Ignore:
                    Console.WriteLine("Kimsenin belasına bulaşmak istemiyorsun. Arkana bakmadan yola devam ettin...");
                    Console.WriteLine("Ana yola çıktığın an hızla gelen bir tır seni ezdi!");
                    Console.WriteLine("[DÖRDÜNCÜ DUVAR] Hiçbir şey yapmamak da bir seçimdir. Ama bu oyunda bedeli ağırdır.");
                    OnPlayerKilledInTraffic?.Invoke();
                    break;
            }
        }
    }
}
