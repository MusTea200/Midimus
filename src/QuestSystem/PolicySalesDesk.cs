using System;
using GameSystems.CharacterSystem;
using GameSystems.RealEstateSystem;
using GameSystems.CoreSystem;

namespace GameSystems.QuestSystem
{

    public class PolicySalesDesk
    {
        private SimulationManager _simManager;
        private Character? _currentClient;
        private DungeonProperty? _currentDungeon;
        private CustomerInteraction? _interaction;
        private Random _rng;

        public PolicySalesDesk(SimulationManager simManager)
        {
            _simManager = simManager;
            _rng = new Random();
        }

        public void StartNegotiation(Character client, DungeonProperty riskTarget)
        {
            _currentClient = client;
            _currentDungeon = riskTarget;
            _interaction = new CustomerInteraction();
            Console.WriteLine($"[SATIŞ MASASI] {_currentClient.Name} adlı müşteri, {_currentDungeon.Name} zindanı için masaya oturdu.");
        }

        public NegotiationResult ProposePremium(int premiumAmount)
        {
            if (_currentClient == null || _currentDungeon == null || _interaction == null)
            {
                throw new InvalidOperationException("Pazarlık başlamadı.");
            }

            // Nusret Etkisi: Reputation yüksekse, tolerans artar.
            int rep = _simManager.GlobalReputation;
            int baseTolerance = _currentDungeon.BaseValue / 2;
            int toleranceLimit = baseTolerance + (baseTolerance * rep / 100);

            // Trait Etkileri
            if (_currentClient.Traits.Contains(TraitType.Generous)) toleranceLimit += 500;
            if (_currentClient.Traits.Contains(TraitType.Thrifty) || _currentClient.Traits.Contains(TraitType.Greedy)) toleranceLimit -= 500;

            if (premiumAmount <= toleranceLimit)
            {
                Console.WriteLine($"[SATIŞ MASASI] Müşteri teklifi KABUL ETTİ. ({premiumAmount} Altın)");
                return NegotiationResult.Accepted;
            }
            else
            {
                // Pazarlık başarısız. Nusret Etkisi ile Satisfaction daha yavaş düşer.
                int dropAmount = 20 - (rep / 10);
                _interaction.PersonalSatisfaction -= Math.Max(5, dropAmount);

                Console.WriteLine($"[SATIŞ MASASI] Müşteri teklifi çok yüksek buldu! Memnuniyet düştü: {_interaction.PersonalSatisfaction}");

                if (_interaction.PersonalSatisfaction <= 0)
                {
                    Console.WriteLine("[SATIŞ MASASI] Müşteri sinirlenip masayı terk etti!");
                    return NegotiationResult.Rejected;
                }

                return NegotiationResult.Rejected;
            }
        }

        public Contract? SignContract(int premium, int payout)
        {
            if (_currentClient == null || _currentDungeon == null || _interaction == null) return null;

            Console.WriteLine($"[SÖZLEŞME İMZALANDI] Prim: {premium}, Tazminat (Payout): {payout}");
            return new Contract(_currentClient, _currentDungeon, premium, payout);
        }
    }
}
