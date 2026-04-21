using UnityEngine;

namespace Visual
{
    public class Cosmos : MonoBehaviour
    {
        [SerializeField] private Transform[] stars;
        [SerializeField] private bool takeChildrenAsStars = true;
        [SerializeField] private float minSpeed = 0.15f;
        [SerializeField] private float maxSpeed = 0.6f;
        [SerializeField] private float teleportFromX = 15f;
        [SerializeField] private float teleportToX = -15f;

        private float[] _starsSpeed;

        private void Start()
        {
            CacheStars();
            GenerateStarsSpeed();
        }

        private void Update()
        {
            if (stars == null || _starsSpeed == null)
                return;

            for (var i = 0; i < stars.Length; i++)
            {
                var star = stars[i];
                if (!star)
                    continue;

                var starPos = star.position;
                starPos.x += _starsSpeed[i] * Time.deltaTime;

                if (starPos.x >= teleportFromX)
                    starPos.x = teleportToX + (starPos.x - teleportFromX);

                star.position = starPos;
            }
        }

        private void CacheStars()
        {
            if (!takeChildrenAsStars || transform.childCount <= 0)
                return;

            stars = new Transform[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
                stars[i] = transform.GetChild(i);
        }

        private void GenerateStarsSpeed()
        {
            if (stars == null)
            {
                _starsSpeed = null;
                return;
            }

            if (maxSpeed < minSpeed)
                (minSpeed, maxSpeed) = (maxSpeed, minSpeed);

            _starsSpeed = new float[stars.Length];
            for (var i = 0; i < _starsSpeed.Length; i++)
                _starsSpeed[i] = Random.Range(minSpeed, maxSpeed);
        }
    }
}