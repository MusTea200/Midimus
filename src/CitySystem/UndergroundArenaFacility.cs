using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class UndergroundArenaFacility : CityFacility
    {
        private DailyLedger _ledger;
        private PoliceManager _police;
        private Random _rng;

        public UndergroundArenaFacility(DailyLedger ledger, PoliceManager police)
            : base("Yeraltı Kolezyumu", 5000)
        {
            _ledger = ledger;
            _police = police;
            _rng = new Random();
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name} karanlık Yeraltı Kolezyumu'na girdi. Kan kokusu havaya hakim.");
        }

        public void EnterTournament(Character c, ArenaLeague league, int betAmount)
        {
            if (_ledger.MainBalance < betAmount)
            {
                Console.WriteLine("Yetersiz bakiye. Bahis yapılamadı.");
                return;
            }

            // Lig Uygunluk Kontrolü
            bool isEligible = false;
            switch (league)
            {
                case ArenaLeague.MechaLeague:
                    isEligible = c.IsCyborg;
                    break;
                case ArenaLeague.MutantLeague:
                    isEligible = c.Mutations.Count > 0;
                    break;
                case ArenaLeague.MonsterLeague:
                    isEligible = (c.Gender == GenderType.MaleMonster || c.Gender == GenderType.FemaleMonster);
                    break;
            }

            if (!isEligible)
            {
                Console.WriteLine($"{c.Name} bu lige uygun değil!");
                return;
            }

            _ledger.DeductBalance(betAmount);

            // Polis Baskını Kontrolü
            if (_rng.NextDouble() <= 0.15)
            {
                Console.WriteLine("SİREN SESLERİ! Polis arenayı bastı!");
                int fine = betAmount * 2; // Ceza
                if (_ledger.MainBalance >= fine) _ledger.DeductBalance(fine);
                else _ledger.DeductBalance(_ledger.MainBalance);

                _police.CommitIllegalAction(2); // Aranma seviyesi artar
                Console.WriteLine($"Polis cezası kesildi: -{fine} Altın.");
                return;
            }

            // Dövüş Simülasyonu
            int strength = c.Attributes.ContainsKey(AttributeType.Strength) ? c.Attributes[AttributeType.Strength] : 10;
            int agility = c.Attributes.ContainsKey(AttributeType.Agility) ? c.Attributes[AttributeType.Agility] : 10;

            float successChance = Math.Clamp((strength + agility) / 200f, 0.1f, 0.9f);

            if (_rng.NextDouble() <= successChance)
            {
                // BAŞARI
                int winnings = betAmount * 3;
                _ledger.AddBalance(winnings);
                c.Stress = 0;
                if (!c.Traits.Contains(TraitType.Gladiator)) c.Traits.Add(TraitType.Gladiator);

                Console.WriteLine($"ZAFER! {c.Name} arenada kan dökerek şampiyon oldu. +{winnings} Altın kazandın! Stres sıfırlandı ve 'Gladyatör' pasifi eklendi.");
            }
            else
            {
                // BAŞARISIZLIK
                c.Stress = 100;
                Console.WriteLine($"MAĞLUBİYET! {c.Name} arenada ağır yaralandı. Bahis ({betAmount} Altın) kaybedildi.");
                // Ölüm eklenebilir
            }
        }
    }
}
