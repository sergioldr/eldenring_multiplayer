using UnityEngine;

namespace SL
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager Instance { get; private set; }

        [Header("Layers")]
        [SerializeField] private LayerMask characterLayers;
        [SerializeField] private LayerMask groundLayers;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }

        public LayerMask GetGroundLayers()
        {
            return groundLayers;
        }
    }
}
