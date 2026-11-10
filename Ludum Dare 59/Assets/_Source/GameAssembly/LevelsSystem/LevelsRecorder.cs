using System.Collections.Generic;
using UnityEngine;

namespace LevelsSystem
{
    public class LevelsRecorder : MonoBehaviour
    {
        public bool IsRestarted;
        private readonly List<int> _completedLevels = new();
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public bool IsLevelCompleted(int level) =>  _completedLevels.Contains(level);
        
        public void MarkLevelAsCompleted(int level)
        {
            if(_completedLevels.Contains(level))
                return;
            
            _completedLevels.Add(level);
        }
    }
}