using UnityEngine;

namespace SL
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera Instance { get; private set; }


        [SerializeField] private Camera cameraObject;
        [SerializeField] private Transform cameraPivotTransform;
        private PlayerManager playerManager;

        [Header("Camera Settings")]
        [SerializeField] private float cameraSmoothSpeed = 1f;
        [SerializeField] private float leftAndRightRotationSpeed = 220f;
        [SerializeField] private float upAndDownRotationSpeed = 220f;
        [SerializeField] private float minimumPivot = -30f;
        [SerializeField] private float maximumPivot = 60f;
        [SerializeField] private float cameraCollisionRadius = 0.2f;
        [SerializeField] private LayerMask collideWithLayers;

        [Header("Camera Values")]
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition;
        [SerializeField] private float leftAndRightLookAngle;
        [SerializeField] private float upAndDownLookAngle;
        private float cameraZPosition;
        private float targetCameraZPosition;

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
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public Camera GetCamera()
        {
            return cameraObject;
        }

        public void SetPlayerManager(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        public void HandleAllCameraActions()
        {
            if (playerManager != null)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraZPosition = Vector3.SmoothDamp(
                transform.position,
                playerManager.transform.position,
                ref cameraVelocity,
                cameraSmoothSpeed * Time.deltaTime
            );

            transform.position = targetCameraZPosition;
        }

        private void HandleRotations()
        {
            leftAndRightLookAngle += PlayerInputManager.Instance.GetCameraHorizontalInput() * leftAndRightRotationSpeed * Time.deltaTime;
            upAndDownLookAngle -= PlayerInputManager.Instance.GetCameraVerticalInput() * upAndDownRotationSpeed * Time.deltaTime;

            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            cameraRotation.y = leftAndRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upAndDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;

            RaycastHit hit;
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
            {
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }
    }
}
