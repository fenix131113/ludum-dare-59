using UnityEngine;

namespace LevelsSystem.Data
{
    [CreateAssetMenu(fileName = "new LevelData", menuName = "SO/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        [field: SerializeField] public int LevelNumber { get; private set; }
        [field: SerializeField] public int NextLevelIndex { get; private set; } = -1;
    }
}