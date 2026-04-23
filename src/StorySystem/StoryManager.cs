using System;
using System.Collections.Generic;

namespace GameSystems.StorySystem
{
    public class StoryManager
    {
        public GlobalStoryState GlobalState { get; private set; }
        private Dictionary<string, StoryNode> _allNodes;

        // Arayüzün abone olacağı, ekrana popup getiren event
        public event Action<StoryNode>? OnStoryTriggered;

        public StoryManager(GlobalStoryState state)
        {
            GlobalState = state;
            _allNodes = new Dictionary<string, StoryNode>();
        }

        public void RegisterNode(StoryNode node)
        {
            if (!_allNodes.ContainsKey(node.NodeID))
            {
                _allNodes.Add(node.NodeID, node);
            }
        }

        // Hikayeyi başlatır/ekrana basar
        public void TriggerNode(string nodeId)
        {
            if (_allNodes.TryGetValue(nodeId, out StoryNode? node) && node != null)
            {
                OnStoryTriggered?.Invoke(node);
            }
            else
            {
                Console.WriteLine($"[Hata] Node bulunamadı: {nodeId}");
            }
        }

        // UI üzerinden bir seçim yapıldığında çağrılır
        public void SelectChoice(StoryNode node, int choiceIndex)
        {
            if (choiceIndex >= 0 && choiceIndex < node.Choices.Count)
            {
                StoryChoice selectedChoice = node.Choices[choiceIndex];

                if (selectedChoice.IsAvailable(GlobalState))
                {
                    // Seçimin sonuçlarını (Action) tetikle
                    selectedChoice.OnSelected?.Invoke();
                }
                else
                {
                    Console.WriteLine("[Hata] Bu seçim için gerekli bayraklara sahip değilsiniz!");
                }
            }
        }
    }
}
