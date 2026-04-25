using System;
using System.Collections.Generic;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;

namespace GameSystems.CitySystem
{
    public class TempleFacility : CityFacility
    {
        private DailyLedger _ledger;

        public TempleFacility(DailyLedger ledger)
            : base("Unutulmuş Şifa Tapınağı", 2000)
        {
            _ledger = ledger;
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name}, gizemli ve karanlık Unutulmuş Şifa Tapınağı'na adım attı.");
            Console.WriteLine("Burada üç farklı bölüm var: Klinik, Bodrum Katı (Richard Gobrigez) ve Şamanın Kulübesi.");
        }

        public void RemoveCurse(Character character, Item item)
        {
            if (!item.IsCursed || !item.IsBound)
            {
                Console.WriteLine("Bu eşya lanetli veya mühürlü değil.");
                return;
            }

            int removalCost = 1500; // Yüksek miktar

            if (_ledger.MainBalance >= removalCost)
            {
                _ledger.DeductBalance(removalCost);
                item.IsBound = false;
                item.IsCursed = false;
                character.EquippedItems.Remove(item);

                Console.WriteLine($"{removalCost} altın bağışlandı. Kutsal ritüel tamamlandı.");
                Console.WriteLine($"{item.ItemName} üzerindeki lanet kırıldı ve eşya paramparça oldu.");

                // Stat geri yükleme mantığı (Basitçe)
                if (item.ItemName == "Kör Öfke Yüzüğü")
                {
                    if (character.Attributes.ContainsKey(AttributeType.Strength)) character.Attributes[AttributeType.Strength] -= 50;
                    Console.WriteLine($"{character.Name}'nin kör edici öfkesi dindi (Güç azaldı, İrade ve Zeka yavaşça geri gelecek).");
                }
            }
            else
            {
                Console.WriteLine($"Laneti kaldırmak için yeterli bağış yok (Gereken: {removalCost}).");
            }
        }



        // 1. Klinik Modülü (Normal İyileşme)
        public void HealHuman(Character character)
        {
            if (character.Gender != GenderType.Male && character.Gender != GenderType.Female)
            {
                Console.WriteLine("Klinik: 'Burada sadece insanları tedavi ediyoruz. Yaratıkları Şaman'a götür.'");
                return;
            }

            int healCost = 300;
            if (_ledger.MainBalance >= healCost)
            {
                _ledger.DeductBalance(healCost);
                character.Stress = 0;

                if (character.Traits.Contains(TraitType.Bruised))
                {
                    character.Traits.Remove(TraitType.Bruised);
                    Console.WriteLine("Klinik: Karakterin fiziksel yaraları iyileştirildi ('Bruised' silindi).");
                }

                Console.WriteLine($"Klinik: {healCost} altın ödendi. {character.Name} tamamen iyileşti ve stresi sıfırlandı.");
            }
            else
            {
                Console.WriteLine("Klinik: Yeterli altınınız yok.");
            }
        }

        // 2. Bodrum Katı (Biyolojik Modifikasyon - Richard Gobrigez)
        public void TalkToRichardGobrigez()
        {
            Console.WriteLine("Richard Gobrigez: 'İnsaniyet zayıflıktır, evlat. Kudrete biat et. Bana ne getirdin?'");
        }

        public void PerformModification(Character character, CraftingMaterial monsterItem)
        {
            TalkToRichardGobrigez();
            Console.WriteLine("Richard Gobrigez tüyler ürpertici bir aletle karakterin üzerine eğiliyor...");

            if (character.Attributes.ContainsKey(AttributeType.Charisma)) character.Attributes[AttributeType.Charisma] -= 20;
            else character.Attributes[AttributeType.Charisma] = -20;

            if (character.Attributes.ContainsKey(AttributeType.Willpower)) character.Attributes[AttributeType.Willpower] -= 20;
            else character.Attributes[AttributeType.Willpower] = -20;

            Console.WriteLine($"{character.Name}'nin insanlığı azaldı: Charisma -20, Willpower -20.");

            if (monsterItem == CraftingMaterial.DragonScale)
            {
                if (character.Attributes.ContainsKey(AttributeType.Endurance)) character.Attributes[AttributeType.Endurance] += 15;
                else character.Attributes[AttributeType.Endurance] = 15;
                Console.WriteLine("MODİFİKASYON: Ejderha Pulu entegre edildi. +15 Endurance!");
            }
            else if (monsterItem == CraftingMaterial.Slime_Core)
            {
                if (character.Attributes.ContainsKey(AttributeType.Agility)) character.Attributes[AttributeType.Agility] += 10;
                else character.Attributes[AttributeType.Agility] = 10;
                Console.WriteLine("MODİFİKASYON: Slime Çekirdeği entegre edildi. +10 Agility!");
            }
            else
            {
                if (character.Attributes.ContainsKey(AttributeType.Strength)) character.Attributes[AttributeType.Strength] += 5;
                else character.Attributes[AttributeType.Strength] = 5;
                Console.WriteLine($"MODİFİKASYON: {monsterItem} entegre edildi. +5 Strength!");
            }
        }



        // --- LABORATUVAR AR-GE VE ALTYAPI SİSTEMİ ---
        public int LabLevel { get; private set; } = 1;
        public System.Collections.Generic.List<GameSystems.CharacterSystem.VatGrownBody> ActiveVats { get; private set; } = new System.Collections.Generic.List<GameSystems.CharacterSystem.VatGrownBody>();
        public System.Collections.Generic.List<GameSystems.CharacterSystem.VatGrownBody> ReadyBodies { get; private set; } = new System.Collections.Generic.List<GameSystems.CharacterSystem.VatGrownBody>();

        public void UpgradeLab()
        {
            TalkToRichardGobrigez();
            int upgradeCost = LabLevel == 1 ? 5000 : 15000;

            if (_ledger.MainBalance >= upgradeCost)
            {
                if (LabLevel >= 3)
                {
                    System.Console.WriteLine("Richard: 'Laboratuvarımız zaten en üst seviyede.'");
                    return;
                }

                _ledger.DeductBalance(upgradeCost);
                LabLevel++;

                System.Console.WriteLine($"AR-GE BAŞARILI! Laboratuvar Seviye {LabLevel} oldu.");
                if (LabLevel == 2) System.Console.WriteLine("Yüksek Riskli klon üretimi (High Risk Vats) kilidi açıldı!");
                if (LabLevel == 3) System.Console.WriteLine("Gelişmiş Soğutma Deposu inşa edildi. Kaset saklama maliyeti düştü.");
            }
            else
            {
                System.Console.WriteLine($"Richard: 'Sermaye eksik evlat. Yükseltme için {upgradeCost} altın gerek.'");
            }
        }

        public void StartGrowingBody(bool isHighRisk)
        {
            TalkToRichardGobrigez();
            if (isHighRisk && LabLevel < 2)
            {
                System.Console.WriteLine("Richard: 'Bu teknoloji için Seviye 2 Laboratuvar şart. Önce yatırım yap.'");
                return;
            }

            int cost = isHighRisk ? 3000 : 1000;
            if (_ledger.MainBalance >= cost)
            {
                _ledger.DeductBalance(cost);
                var newVat = new GameSystems.CharacterSystem.VatGrownBody(isHighRisk);
                ActiveVats.Add(newVat);
                System.Console.WriteLine($"{newVat.VatId} üretim bandına alındı. Büyüme süreci başladı.");
            }
            else
            {
                System.Console.WriteLine($"Richard: 'Üretim maliyeti ({cost} altın) için yeterli bütçemiz yok.'");
            }
        }

        // --- SİBER ENTEGRASYONLAR ---
        public void InstallBrainChip(Character character)
        {
            TalkToRichardGobrigez();
            if (character.HasBrainChip)
            {
                Console.WriteLine("Richard: 'Bu deneğin zaten bir çipi var.'");
                return;
            }

            character.IsCyborg = true;
            character.HasBrainChip = true;

            if (character.Attributes.ContainsKey(AttributeType.Charisma)) character.Attributes[AttributeType.Charisma] = 0;
            else character.Attributes.Add(AttributeType.Charisma, 0);

            if (character.Attributes.ContainsKey(AttributeType.Willpower)) character.Attributes[AttributeType.Willpower] = 0;
            else character.Attributes.Add(AttributeType.Willpower, 0);

            character.Stress = 0;
            Console.WriteLine($"{character.Name}'ye Beyin Çipi takıldı! Maksimum Stres etkisiz hale geldi. Ancak Charisma ve Willpower 0'landı.");
        }

        // --- BİYOLOJİK MUTASYONLAR ---
        public void ApplyChemicalBath(Character character)
        {
            TalkToRichardGobrigez();
            Console.WriteLine("Karakter kimyasal havuzuna atıldı...");

            Random rng = new Random();
            Array mutations = Enum.GetValues(typeof(MutationType));
            MutationType randomMutation = (MutationType)mutations.GetValue(rng.Next(mutations.Length));

            character.Mutations.Add(randomMutation);

            if (randomMutation == MutationType.SlimeArm)
            {
                if (character.Attributes.ContainsKey(AttributeType.Strength)) character.Attributes[AttributeType.Strength] += 30;
                else character.Attributes.Add(AttributeType.Strength, 30);
                if (character.Attributes.ContainsKey(AttributeType.Agility)) character.Attributes[AttributeType.Agility] -= 15;
                else character.Attributes.Add(AttributeType.Agility, -15);
                Console.WriteLine("MUTASYON: Slime Kolu! (+30 Strength, -15 Agility)");
            }
            else if (randomMutation == MutationType.BeastLegs)
            {
                if (character.Attributes.ContainsKey(AttributeType.Agility)) character.Attributes[AttributeType.Agility] += 30;
                else character.Attributes.Add(AttributeType.Agility, 30);
                if (character.Attributes.ContainsKey(AttributeType.Intelligence)) character.Attributes[AttributeType.Intelligence] -= 15;
                else character.Attributes.Add(AttributeType.Intelligence, -15);
                Console.WriteLine("MUTASYON: Canavar Bacakları! (+30 Agility, -15 Intelligence)");
            }
            else
            {
                if (character.Attributes.ContainsKey(AttributeType.Endurance)) character.Attributes[AttributeType.Endurance] += 30;
                else character.Attributes.Add(AttributeType.Endurance, 30);
                if (character.Attributes.ContainsKey(AttributeType.Charisma)) character.Attributes[AttributeType.Charisma] -= 20;
                else character.Attributes.Add(AttributeType.Charisma, -20);
                Console.WriteLine("MUTASYON: Asit Kan! (+30 Endurance, -20 Charisma)");
            }
        }

        public void StartCocoonPhase(Character character, int currentDay)
        {
            TalkToRichardGobrigez();
            if (character.IsLockedInCocoon)
            {
                Console.WriteLine("Richard: 'Denek zaten kozada.'");
                return;
            }

            character.IsLockedInCocoon = true;
            character.CocoonEntryDay = currentDay;

            Console.WriteLine($"{character.Name} biyolojik bir kozaya hapsedildi. 7 gün boyunca kullanılamaz.");
        }

        // --- KİMLİK VE RUH MANİPÜLASYONU ---
        public void PerformSpeciesTransition(Character character)
        {
            TalkToRichardGobrigez();
            if (character.Gender == GenderType.Male) character.Gender = GenderType.MaleMonster;
            else if (character.Gender == GenderType.Female) character.Gender = GenderType.FemaleMonster;
            else if (character.Gender == GenderType.MaleMonster) character.Gender = GenderType.Male;
            else if (character.Gender == GenderType.FemaleMonster) character.Gender = GenderType.Female;

            Console.WriteLine($"{character.Name} tür değişimi geçirdi! Yeni türü: {character.Gender}");
        }

        public GameSystems.CharacterSystem.MindVHSTape ExtractConsciousnessToVHS(GameSystems.CharacterSystem.Character source)
        {
            TalkToRichardGobrigez();
            int goldCost = 5000;
            if (_ledger.MainBalance < goldCost)
            {
                System.Console.WriteLine($"Yetersiz altın. VHS aktarımı için {goldCost} altın gerekiyor.");
                return null;
            }
            _ledger.DeductBalance(goldCost);

            System.Console.WriteLine($"{source.Name}'nin zihni yasadışı bir VHS kasete aktarılıyor...");
            GameSystems.CharacterSystem.MindVHSTape tape = new GameSystems.CharacterSystem.MindVHSTape(source.Name, source.Traits, source.AdvancedTraits, source.Attributes);

            System.Console.WriteLine("Zihin kopyalandı. Kaynak beden klinik olarak ölü duruma getirildi (Kalıcı Ölüm).");
            source.Stress = 100;
            source.Traits.Clear();
            return tape;
        }

        public void ImplantVHSIntoBody(GameSystems.CharacterSystem.MindVHSTape tape, GameSystems.CharacterSystem.VatGrownBody targetBody, GameSystems.CharacterSystem.Character newCharacterInstance)
        {
            TalkToRichardGobrigez();
            if (!targetBody.IsReady || targetBody.IsDestroyed)
            {
                System.Console.WriteLine("Richard: 'Bu beden henüz hazır değil veya çürümüş.'");
                return;
            }

            int goldCost = 10000;
            if (_ledger.MainBalance < goldCost)
            {
                System.Console.WriteLine($"Yasadışı zihin enjeksiyonu operasyonu için {goldCost} altın gerekiyor.");
                return;
            }
            _ledger.DeductBalance(goldCost);

            System.Console.WriteLine($"'{tape.OriginalName}' VHS kaseti yeni klon bedenine ({targetBody.VatId}) entegre ediliyor...");

            newCharacterInstance.Traits.Clear();
            newCharacterInstance.AdvancedTraits.Clear();

            newCharacterInstance.Traits.AddRange(tape.Traits);
            newCharacterInstance.AdvancedTraits.AddRange(tape.AdvancedTraits);
            newCharacterInstance.Traits.AddRange(targetBody.BaseTraits);

            // Bedenin genetik mutasyon statları ile kasetin tecrübe statlarını harmanla
            foreach (var kvp in targetBody.BaseAttributes)
            {
                if (newCharacterInstance.Attributes.ContainsKey(kvp.Key)) newCharacterInstance.Attributes[kvp.Key] = kvp.Value;
                else newCharacterInstance.Attributes.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in tape.Attributes)
            {
                // Tecrübeler fiziksel sınırlarla birleşiyor (Basit toplama)
                if (newCharacterInstance.Attributes.ContainsKey(kvp.Key)) newCharacterInstance.Attributes[kvp.Key] += (kvp.Value / 2); // Kasetten gelen statların yarısı
                else newCharacterInstance.Attributes.Add(kvp.Key, kvp.Value / 2);
            }

            newCharacterInstance.AdvancedTraits.Add(new GameSystems.CharacterSystem.BodyAcclimatizationTrait(20));
            ReadyBodies.Remove(targetBody);

            System.Console.WriteLine($"İşlem başarılı! Yeni klon bedeni ayaklandı. Beden disforisi (Acclimatization) başladı. Beden 20 gün boyunca polis riski taşıyacak.");
        }

        // 3. Şamanın Kulübesi (Canavar Şifası ve Büyü)
        public void TalkToShaman()
        {
            Console.WriteLine("Şaman: 'Doğanın ruhları ve vahşi kanın yankısı... Ne istiyorsun yabancı?'");
        }

        public void HealMonster(Character character)
        {
            if (character.Gender != GenderType.MaleMonster && character.Gender != GenderType.FemaleMonster)
            {
                Console.WriteLine("Şaman: 'Senin kanın çok zayıf, insan. Bu büyüleri kaldıramazsın.'");
                return;
            }

            character.Stress = 0;
            Console.WriteLine($"Şaman: Vahşi ruhlar {character.Name}'yi arındırdı. Stres sıfırlandı.");
        }

        public Item? CraftSpell(List<CraftingMaterial> components)
        {
            TalkToShaman();
            if (components.Count < 2)
            {
                Console.WriteLine("Şaman: 'Büyü yapmak için en az 2 materyale ihtiyacım var.'");
                return null;
            }

            Console.WriteLine("Şaman gizemli sözler mırıldanarak materyalleri kazanda kaynatıyor...");
            Item spellItem = new Item("Şamanın Tek Kullanımlık Görev Büyüsü", 50);
            Console.WriteLine("YENİ EŞYA ÜRETİLDİ: Şamanın Tek Kullanımlık Görev Büyüsü!");
            return spellItem;
        }
    }
}
