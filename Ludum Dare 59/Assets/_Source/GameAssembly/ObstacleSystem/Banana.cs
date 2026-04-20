using UnityEngine;
using Utils;

namespace ObstacleSystem
{
    public class Banana : MonoBehaviour
    {
        [SerializeField] private LayerMask triggerLayers;
        [SerializeField] private float speed;
        [SerializeField] private float destroyTime;
        [SerializeField] private float triggerZoneMinDistance;
        [SerializeField] private GameObject bananaPeel;

        private Vector2 _targetPos;

        private void Start()
        {
            Destroy(gameObject, destroyTime);
        }

        private void Update()
        {
            if(Vector2.Distance(transform.position, _targetPos) <= triggerZoneMinDistance)
            {
                DropBanana();
                return;
            }
            
            transform.position += ((Vector3)_targetPos - transform.position).normalized * (Time.deltaTime * speed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!LayerService.CheckLayersEquality(other.gameObject.layer, triggerLayers))
                return;

            DropBanana();
        }

        public void Initialize(Vector2 targetPos)
        {
            _targetPos = targetPos;
        }

        private void DropBanana()
        {
            Instantiate(bananaPeel, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}