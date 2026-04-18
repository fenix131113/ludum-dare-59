using UnityEngine;

namespace Utils
{
    public class TempParticleSource : MonoBehaviour
    {
        [SerializeField] public float destroyTime;

        public void Init(float overrideDestroyTime = -1)
        {
            if(!Mathf.Approximately(overrideDestroyTime, -1))
                destroyTime = overrideDestroyTime;
        }

        private void Start() => Destroy(gameObject, destroyTime);

        private void OnEnable() => Start();
    }
}