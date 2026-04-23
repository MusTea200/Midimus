using System;
using GameSystems.CharacterSystem;
using GameSystems.CoreSystem;
using GameSystems.StorySystem;

namespace GameSystems.CitySystem
{
    public class BlacksmithFacility : CityFacility
    {
        private GlobalStoryState _storyState;
        private PlayerManager _player;
        private DailyLedger _ledger;

        public BlacksmithFacility(GlobalStoryState state, PlayerManager player, DailyLedger ledger)
            : base("Demirci", 1000)
        {
            _storyState = state;
            _player = player;
            _ledger = ledger;
        }

        public override void EnterFacility(Character character)
        {
            Console.WriteLine($"{character.Name}, Demirci'ye giriş yaptı.");
            // Arayüz tetikleyicisi veya hoşgeldin diyaloğu burada çalışabilir.
        }

        public void CraftItem(Character character, CraftingMaterial materialNeeded, int materialCost, int goldCost)
        {
            float discountMultiplier = 1.0f;

            if (_storyState.HasFlag("Blacksmith_Unlocked") && _storyState.HasFlag("Crafting_Discount"))
            {
                discountMultiplier = 0.5f; // Frank'in kitabından gelen %50 indirim
                Console.WriteLine("Demirci Frank'in teknikleri sayesinde üretim maliyeti yarı yarıya düştü!");
            }

            int finalMaterialCost = (int)(materialCost * discountMultiplier);
            int finalGoldCost = (int)(goldCost * discountMultiplier);

            if (_ledger.MainBalance >= finalGoldCost && _player.ConsumeMaterial(materialNeeded, finalMaterialCost))
            {
                _ledger.DeductBalance(finalGoldCost);
                Console.WriteLine($"{character.Name} için {materialNeeded} kullanılarak yeni bir eşya üretildi.");
                // Karakterin statlarına küçük bir buff eklenebilir
            }
            else
            {
                Console.WriteLine("Yeterli altın veya materyal yok.");
            }
        }

        public void CraftExcavator(Character character)
        {
            if (!_storyState.HasFlag("Excavator_Blueprint"))
            {
                Console.WriteLine("Ekskavatör (Excavator) planına sahip değilsiniz!");
                return;
            }

            if (character.Stress > 0)
            {
                Console.WriteLine("Bu efsanevi silahı kuşanmak için karakterin tamamen stressiz (0 Stres) ve odaklanmış olması gerekir.");
                return;
            }

            int ironCost = 50;
            int dragonCost = 10;

            if (_player.HasMaterial(CraftingMaterial.IronOre, ironCost) &&
                _player.HasMaterial(CraftingMaterial.DragonScale, dragonCost))
            {
                _player.ConsumeMaterial(CraftingMaterial.IronOre, ironCost);
                _player.ConsumeMaterial(CraftingMaterial.DragonScale, dragonCost);

                Console.WriteLine($"EFSANEVİ SİLAH: {character.Name} artık Excavator'ı kuşanıyor!");
                // Kalıcı stat buff'ı (Örnek: Güç statüsünü kalıcı olarak 50 artır)
                if (character.Attributes.ContainsKey(AttributeType.Strength))
                {
                    character.Attributes[AttributeType.Strength] += 50;
                }
            }
            else
            {
                Console.WriteLine("Excavator'ı üretmek için yeterli IronOre (50) veya DragonScale (10) yok.");
            }
        }
    }
}
