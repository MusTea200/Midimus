using System;
using System.Collections.Generic;
using System.Linq;

namespace GameSystems.StorySystem
{
    public class StoryChoice
    {
        public string ChoiceText { get; private set; }
        public List<string> RequiredFlags { get; private set; }
        public Action OnSelected { get; private set; }

        public StoryChoice(string text, Action onSelected, List<string>? requiredFlags = null)
        {
            ChoiceText = text;
            OnSelected = onSelected;
            RequiredFlags = requiredFlags ?? new List<string>();
        }

        // Seçeneğin oyuncuya görünüp görünmeyeceğini denetler
        public bool IsAvailable(GlobalStoryState state)
        {
            if (RequiredFlags == null || RequiredFlags.Count == 0) return true;
            return RequiredFlags.All(flag => state.HasFlag(flag));
        }
    }

    public class StoryNode
    {
        public string NodeID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public List<StoryChoice> Choices { get; private set; }

        public StoryNode(string id, string title, string description)
        {
            NodeID = id;
            Title = title;
            Description = description;
            Choices = new List<StoryChoice>();
        }

        public void AddChoice(StoryChoice choice)
        {
            Choices.Add(choice);
        }
    }
}
