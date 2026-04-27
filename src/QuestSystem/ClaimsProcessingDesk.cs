using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.QuestSystem
{
    public enum ClaimQuestionType
    {
        UsedHealingPotions,
        IgnoredWarningSigns,
        FriendlyFireInvolved
    }

    public class ClaimsProcessingDesk
    {
        private SimulationManager _simManager;
        private Contract? _activeContract;
        private CustomerInteraction? _interaction;
        private Random _rng;

        public ClaimsProcessingDesk(SimulationManager simManager)
        {
            _simManager = simManager;
            _rng = new Random();
        }

        public void FileClaim(Contract activePolicy)
        {
            _activeContract = activePolicy;
            _interaction = new CustomerInteraction();
            _interaction.PersonalSatisfaction = 80; // Tazminat almaya geldikleri için genelde gergindirler
            Console.WriteLine($"[TAZMİNAT MASASI] {_activeContract.Client.Name} hasarlı döndü ve tazminat ({_activeContract.Payout} Altın) talep ediyor.");
        }

        public void CrossExamine(ClaimQuestionType question, int officeInvestigationStat)
        {
            if (_activeContract == null || _interaction == null)
            {
                throw new InvalidOperationException("Aktif bir tazminat dosyası yok.");
            }

            Character client = _activeContract.Client;
            int clientWillpower = client.Attributes.ContainsKey(AttributeType.Willpower) ? client.Attributes[AttributeType.Willpower] : 10;

            // Oyuncu Statı vs Müşteri İradesi (RNG Roll)
            int playerRoll = _rng.Next(1, 21) + (officeInvestigationStat / 5);
            int clientRoll = _rng.Next(1, 21) + (clientWillpower / 5);

            Console.WriteLine($"[SORGU] {question} üzerinden çapraz sorgu yapılıyor... (Zar: {playerRoll} vs {clientRoll})");

            if (playerRoll > clientRoll)
            {
                // Başarılı Sorgu (Ace Attorney misali yalanı yakalandı)
                int originalPayout = _activeContract.Payout;
                _activeContract.Payout = (int)(originalPayout * 0.20f); // %80 düşer
                _interaction.PersonalSatisfaction -= 40;

                Console.WriteLine("[İTİRAZ KABUL EDİLDİ!] Müşterinin yalanı yakalandı! Tazminat bedeli %80 oranında düşürüldü.");
                Console.WriteLine($"Yeni Ödenecek Tutar: {_activeContract.Payout} Altın.");
            }
            else
            {
                // Başarısız Sorgu
                _interaction.PersonalSatisfaction -= 10;
                Console.WriteLine("[İTİRAZ REDDEDİLDİ!] Müşteri haklı çıktı, köşeye sıkıştıramadın.");
            }

            if (_interaction.PersonalSatisfaction <= 0)
            {
                Console.WriteLine("[TAZMİNAT MASASI KAVGA!] Müşteri sinir krizi geçirdi, sözleşmeyi yırttı attı ve şirketi dava etmekle tehdit etti.");
                _activeContract.IsActive = false;
                _simManager.GlobalReputation -= 15;
                Console.WriteLine("Şirketin Prestiji (GlobalReputation) kalıcı hasar aldı!");
            }
        }
    }
}
