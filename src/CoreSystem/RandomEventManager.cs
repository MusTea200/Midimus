using System;
using System.Collections.Generic;

namespace GameSystems.CoreSystem
{
    public abstract class GameEvent
    {
        public string EventID { get; private set; }
        public bool IsTriggered { get; protected set; }

        // Olay tetiklendiğinde UI tarafında bir popup çıkmasını veya arkaplan işlemini başlatacak Event
        public event Action<GameEvent>? OnEventTriggered;

        protected GameEvent(string id)
        {
            EventID = id;
            IsTriggered = false;
        }

        // Koşulların uygun olup olmadığını kontrol eder (Manager tarafından çağrılır)
        public abstract bool CheckConditions();

        // Olayı tetikler ve sonucu uygular
        public virtual void TriggerEvent()
        {
            if (IsTriggered) return;

            IsTriggered = true;
            Console.WriteLine($"Olay Tetiklendi: {EventID}");
            OnEventTriggered?.Invoke(this);
            ApplyEffects();
        }

        // Alt sınıflar kendi spesifik sonuçlarını burada tanımlar
        protected abstract void ApplyEffects();
    }

    public class RandomEventManager
    {
        private List<GameEvent> _availableEvents;
        private Random _rng;

        public RandomEventManager()
        {
            _availableEvents = new List<GameEvent>();
            _rng = new Random();
        }

        public void RegisterEvent(GameEvent newEvent)
        {
            _availableEvents.Add(newEvent);
        }

        // Örneğin gün başlarken veya biterken çağrılarak rastgele olayları kışkırtır
        public void EvaluateRandomEvents()
        {
            foreach (var gameEvent in _availableEvents)
            {
                if (!gameEvent.IsTriggered && gameEvent.CheckConditions())
                {
                    // %10 ihtimalle olay gerçekleşsin (Kelebek etkisi)
                    if (_rng.NextDouble() <= 0.10)
                    {
                        gameEvent.TriggerEvent();
                        break; // Aynı anda birden fazla büyük olay olmasını engellemek için
                    }
                }
            }
        }
    }
}
