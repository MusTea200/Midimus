using System;
using GameSystems.CharacterSystem;

namespace GameSystems.CoreSystem
{
    public enum DialogueMood
    {
        Normal,
        Hostile,
        Psychotic
    }

    public class SanityDistortionManager
    {
        private SimulationManager _simManager;
        private Random _rng;
        private bool _darkOptionsUnlocked;

        public event Action? OnScreenGlitchTriggered;
        public event Action<Character>? OnCreepyNPCBehavior;
        public event Action? OnDarkDialogueOptionsUnlocked;

        public SanityDistortionManager(SimulationManager simManager)
        {
            _simManager = simManager;
            _rng = new Random();
            _darkOptionsUnlocked = false;
        }

        public void CheckForDistortions(Character? currentNpc = null)
        {
            int karma = _simManager.HiddenKarmaScore;

            if (karma <= -50 && !_darkOptionsUnlocked)
            {
                _darkOptionsUnlocked = true;
                OnDarkDialogueOptionsUnlocked?.Invoke();
            }

            if (karma <= -100)
            {
                // Ağır halüsinasyonlar ve etkiler
                if (_rng.NextDouble() <= 0.03)
                {
                    OnScreenGlitchTriggered?.Invoke();
                }

                if (currentNpc != null && _rng.NextDouble() <= 0.05)
                {
                    OnCreepyNPCBehavior?.Invoke(currentNpc);
                }
            }
            else if (karma <= -50)
            {
                // Hafif belirtiler
                if (_rng.NextDouble() <= 0.01)
                {
                    OnScreenGlitchTriggered?.Invoke();
                }
            }
        }

        public DialogueMood GetDialogueMood()
        {
            int karma = _simManager.HiddenKarmaScore;

            if (karma <= -100)
            {
                return DialogueMood.Psychotic;
            }
            else if (karma <= -50)
            {
                return DialogueMood.Hostile;
            }

            return DialogueMood.Normal;
        }
    }
}
