using System;
using GameSystems.CharacterSystem;

namespace GameSystems.QuestSystem
{
    public enum NegotiationResult
    {
        Accepted,
        Rejected,
        WalkedAway
    }

    public class NegotiationSession
    {
        public Character Customer { get; private set; }
        public InsurancePolicy ProposedPolicy { get; private set; }

        public NegotiationSession(Character character, InsurancePolicy policy)
        {
            Customer = character;
            ProposedPolicy = policy;
        }

        // Oyuncu teklif sunduğunda çağrılır
        public NegotiationResult SubmitOffer()
        {
            // Yan etkileri (stres, patience vb.) sadece teklif sunulduğunda uygula
            ProposedPolicy.ApplyPolicyEffects();

            // Slider 1-100 arası (50 dengeli, >50 oyuncuya kârlı, <50 karaktere kârlı)
            // Oran olarak (0.0f - 1.0f) çevirelim:
            float proposedRatio = ProposedPolicy.SliderValue / 100f;

            // Aradaki farkı hesapla (Karakter düşük oranları yani kendine kârlı olanı ister)
            float difference = proposedRatio - Customer.IdealOfferRatio;

            // Kabul aralığı: Karakterin toleransını stres ve sabır belirler
            float tolerance = 0.1f + (Customer.Patience * 0.05f) - (Customer.Stress / 500f);

            // Fobisi olan karakterler sigorta anlaşmalarına daha güvensiz ve zor ikna olurlar
            if (Customer.Phobias.Count > 0)
            {
                tolerance -= (Customer.Phobias.Count * 0.05f);
            }

            if (difference <= tolerance)
            {
                // Teklif karakter için uygun
                Console.WriteLine($"{Customer.Name} teklifi kabul etti.");
                return NegotiationResult.Accepted;
            }
            else
            {
                // Teklif reddedildi, sabır düşer
                Customer.Patience -= 1;
                Customer.ApplyInteraction(InteractionType.BadInsuranceOffer);

                if (Customer.Patience <= 0 || Customer.Stress > 85)
                {
                    Console.WriteLine($"{Customer.Name} masadan kalktı. Anlaşma iptal.");
                    return NegotiationResult.WalkedAway;
                }
                else
                {
                    Console.WriteLine($"{Customer.Name} teklifi reddetti. Kendi çıkarına daha uygun bir anlaşma istiyor. Sabır: {Customer.Patience}");
                    return NegotiationResult.Rejected;
                }
            }
        }
    }
}
