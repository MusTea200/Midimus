using System;
using System.Collections.Generic;
using GameSystems.CoreSystem;

namespace GameSystems.CharacterSystem
{
    public enum AttributeType
    {
        Strength, Agility, Intelligence, Luck, Endurance, Charisma, Perception, Willpower
    }


    public enum GenderType
    {
        Male, Female, MaleMonster, FemaleMonster
    }


    public enum TraitType
    {
        Brave, Cowardly, Greedy, Generous, Gluttonous, Irritable, Thrifty, HasPet, Cautious, Childish, Santas_Blessing, Broken_Heart, Iron_Liver, Hangover, Brawler, Bruised, Cowardly_Fast
    }


    public enum PhobiaType
    {
        Claustrophobia, // Kapalı alan korkusu (Zindan)
        Arachnophobia,  // Örümcek korkusu
        Nyctophobia,    // Karanlık korkusu
        Hemophobia,     // Kan korkusu
        Pyrophobia      // Ateş korkusu
    }


    public enum InteractionType
    {
        ForceDiet, HardTraining, ForceSocialize, OfferRest, GiveFavoriteFood, BadInsuranceOffer
    }


    public class Character
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public GenderType Gender { get; private set; }
        public Dictionary<AttributeType, int> Attributes { get; private set; } // 1-100 arası
        public List<TraitType> Traits { get; private set; }
        public List<UpgradeableTrait> AdvancedTraits { get; private set; }
        public List<PhobiaType> Phobias { get; private set; }
        public List<Item> EquippedItems { get; private set; }
        public Dictionary<InteractionType, bool> RedLines { get; private set; } // True: Kırmızı çizgi, False: Sevdiği eylem

        public bool MiracleReady { get; set; }
        public int Exp { get; set; }

        public Mount? ActiveMount { get; set; }

        private int _stress;
        public int Stress
        {
            get => _stress;
            set
            {
                _stress = Math.Clamp(value, -100, 100);
                CheckPsychologicalThresholds();
            }
        }

        private void CheckPsychologicalThresholds()
        {
            if (_stress >= 90)
            {
                Array values = Enum.GetValues(typeof(PhobiaType));
                object? randomValue = values.GetValue(new Random().Next(values.Length));
                if (randomValue == null) return;

                PhobiaType randomPhobia = (PhobiaType)randomValue;

                if (!Phobias.Contains(randomPhobia))
                {
                    Phobias.Add(randomPhobia);
                    Console.WriteLine($"{Name} aşırı stresten ötürü yeni bir fobi kazandı: {randomPhobia}!");
                    _stress = 50; // Krizi atlattıktan sonra stres biraz dengelenir
                }
            }
            else if (_stress <= -90)
            {
                if (!MiracleReady)
                {
                    MiracleReady = true;
                    Console.WriteLine($"{Name} aşırı sadakat ve huzur hissiyle bir 'Mucize' (MiracleReady) kazandı!");
                    _stress = -50; // Mucize kazanımı sonrası durum dengelenir
                }
            }
        }

        private int _patience;
        public int Patience
        {
            get => _patience;
            set => _patience = Math.Clamp(value, 0, 100);
        }

        // Uyum seviyesi: Stres arttıkça oyuncunun kararlarına itaati düşer (0.0f - 1.0f)
        public float Compliance => 1.0f - (Stress / 100f);

        // Karakterin istediği ideal kâr/zarar oranı (0.0 - 1.0 arası bir değer olarak tutulabilir, ancak Slider üzerinden hesaplanacak)
        public float IdealOfferRatio { get; set; } = 0.5f;

        public Character(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Array genderValues = Enum.GetValues(typeof(GenderType));
            Gender = (GenderType)genderValues.GetValue(new Random().Next(genderValues.Length))!;

            Attributes = new Dictionary<AttributeType, int>();
            Traits = new List<TraitType>();
            AdvancedTraits = new List<UpgradeableTrait>();
            Phobias = new List<PhobiaType>();
            EquippedItems = new List<Item>();
            RedLines = new Dictionary<InteractionType, bool>();
            Stress = 0;
            Exp = 0;
            MiracleReady = false;
            Patience = new Random().Next(2, 6); // 2-5 arası rastgele sabır

            // Yetenekleri varsayılan olarak başlat
            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
                Attributes[attr] = new Random().Next(10, 50);
        }

        // Oyuncunun karakterle girdiği etkileşimi çözümleyen fonksiyon
        public void ApplyInteraction(InteractionType interaction)
        {
            if (RedLines.TryGetValue(interaction, out bool isRedLine))
            {
                if (isRedLine) // Karakterin kırmızı çizgisine basıldı (Örn: Obura zorla diyet yaptırmak)
                {
                    Stress += 30;
                    HandleAggressiveBehavior();
                }
                else // Karakterin sevdiği eylem yapıldı
                {
                    Stress -= 20;
                }
            }
        }

        private void HandleAggressiveBehavior()
        {
            if (Stress > 70)
            {
                // Rastgele karar tetikleyicisi (Örn: Göreve gitmeyi reddetme, eşya kırma, zam isteme)
                Console.WriteLine($"{Name} çok stresli! Otoriteye karşı geliyor...");
            }
        }



        public void EquipItem(Item item)
        {
            if (item.IsCursed && item.IsUnidentified)
            {
                item.IsBound = true;
                Console.WriteLine($"MÜHÜRLENDİ! {item.ItemName} lanetli çıktı ve {Name}'in üzerine mühürlendi (Çıkarılamaz).");

                if (item.ItemName == "Kör Öfke Yüzüğü")
                {
                    if (Attributes.ContainsKey(AttributeType.Strength)) Attributes[AttributeType.Strength] += 50;
                    else Attributes[AttributeType.Strength] = 50;
                    Attributes[AttributeType.Willpower] = 0;
                    Attributes[AttributeType.Intelligence] = 0;
                    Console.WriteLine($"{Name} kör edici bir öfkeyle doldu! (+50 Strength, Willpower & Intelligence 0 oldu).");
                }
                else if (item.ItemName == "Midas'ın Prangası")
                {
                    Attributes[AttributeType.Charisma] = 0;
                    Attributes[AttributeType.Luck] = 0;
                    Console.WriteLine($"{Name} Midas'ın Prangası'na takıldı! (Charisma & Luck 0 oldu).");
                }
            }
            EquippedItems.Add(item);
        }
    }
}
