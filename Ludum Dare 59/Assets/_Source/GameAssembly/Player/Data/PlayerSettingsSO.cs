using UnityEngine;

namespace Player.Data
{
    [CreateAssetMenu(fileName = "new PlayerSettingsSO", menuName = "SO/Player Settings")]
    public class PlayerSettingsSO : ScriptableObject
    {
        [field: SerializeField] public float MoveSpeed { get; private set; }
    }
}