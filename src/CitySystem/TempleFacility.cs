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
