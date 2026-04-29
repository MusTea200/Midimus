using System.Collections.Generic;

namespace GameSystems.StorySystem
{
    public class GlobalStoryState
    {
        private HashSet<string> _activeFlags;

        public GlobalStoryState()
        {
            _activeFlags = new HashSet<string>();
            DiscoveredOtherUniverses = false;
        }

        public void AddFlag(string flagName)
        {
            _activeFlags.Add(flagName);
        }

        public bool HasFlag(string flagName)
        {
            return _activeFlags.Contains(flagName);
        }

        public bool DiscoveredOtherUniverses { get; set; }

        public void RemoveFlag(string flagName)
        {
            _activeFlags.Remove(flagName);
        }
    }
}
