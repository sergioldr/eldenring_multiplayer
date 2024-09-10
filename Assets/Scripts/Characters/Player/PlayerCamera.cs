using UnityEngine;

namespace SL
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera Instance { get; private set; }

        [SerializeField] private Camera cameraObject;

        [Header("Camera Settings")]
        private Vector3 cameraVelocity;

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

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public Camera GetCamera()
        {
            return cameraObject;
        }

        private void HandleAllCameraActions()
        {

        }

        private void FollowTarget()
        {

        }
    }
}
