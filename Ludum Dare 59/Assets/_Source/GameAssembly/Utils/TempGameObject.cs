using UnityEngine;

namespace Utils
{
    public class TempGameObject : MonoBehaviour
    {
        [SerializeField] private bool startDestroyAwake;
        [SerializeField] private float destroyTime;

        private void Awake()
        {
            if(startDestroyAwake)
                Destroy(gameObject, destroyTime);
        }

        public void StartDestroy(float overrideTime = float.MinValue)
        {
            if(!overrideTime.Equals(float.MinValue))
                destroyTime = overrideTime;
            
            Destroy(gameObject, destroyTime);
        }
    }
}