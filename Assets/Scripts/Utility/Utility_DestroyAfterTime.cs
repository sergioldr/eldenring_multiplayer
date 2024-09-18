using UnityEngine;

namespace SL
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float timeToDestroy = 5f;

        private void Awake()
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}
