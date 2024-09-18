using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

        [Header("Lock On")]
        [SerializeField] private float lockOnRadius = 20f;
        [SerializeField] private float minimumViewableAngle = -50f;
        [SerializeField] private float maximumViewableAngle = 50f;
        [SerializeField] private float lockOnTargetFollowSpeed = 0.2f;
        [SerializeField] private float unlockedCameraHeight = 1.65f;
        [SerializeField] private float lockedCameraHeight = 2.0f;
        [SerializeField] private float setCameraHeightSpeed = 0.05f;
        public CharacterManager nearestLockOnTarget { get; private set; }
        public CharacterManager rightLockOnTarget { get; private set; }
        public CharacterManager leftLockOnTarget { get; private set; }
        private List<CharacterManager> availableTargets = new List<CharacterManager>();
        private Coroutine cameraLockOnHeightCoroutine;

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
            if (playerManager.GetPlayerNetworkManager().isLockedOn.Value)
            {
                // HANDLE LOCK ON ROTATIONS

                // THIS ROTATES THIS GAMEOBJECT
                Vector3 rotationDirection = playerManager.GetPlayerCombatManager().currentTarget.GetCharacterCombatManager().lockOnTransform.position - transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

                // THIS ROTATES THE PIVOT OBJECT
                rotationDirection = playerManager.GetPlayerCombatManager().currentTarget.GetCharacterCombatManager().lockOnTransform.position - cameraPivotTransform.position;
                rotationDirection.Normalize();

                targetRotation = Quaternion.LookRotation(rotationDirection);
                cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

                leftAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = cameraPivotTransform.eulerAngles.x;
            }
            else
            {
                // HANDLE FREE ROTATIONS

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

        public void HandleLocatingLockOnTargets()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;


            Collider[] colliders = Physics.OverlapSphere(playerManager.transform.position, lockOnRadius, WorldUtilityManager.Instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                if (lockOnTarget != null)
                {
                    // CHECK IF THEY ARE WITHIN OUR FIELD OF VIEW
                    Vector3 lockOnTargetDirection = lockOnTarget.transform.position - playerManager.transform.position;
                    float distanceFromTarget = Vector3.Distance(playerManager.transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);

                    if (lockOnTarget.GetIsDead())
                    {
                        continue;
                    }

                    if (lockOnTarget.transform.root == playerManager.transform.root)
                    {
                        continue;
                    }

                    if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                    {
                        RaycastHit hit;

                        if (Physics.Linecast(playerManager.GetPlayerCombatManager().lockOnTransform.position,
                            lockOnTarget.GetCharacterCombatManager().lockOnTransform.position,
                            out hit,
                            WorldUtilityManager.Instance.GetGroundLayers())
                        )
                        {
                            continue;
                        }
                        else
                        {
                            availableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }

            for (int k = 0; k < availableTargets.Count; k++)
            {
                if (availableTargets[k] != null)
                {
                    float distanceFromTarget = Vector3.Distance(playerManager.transform.position, availableTargets[k].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[k];
                    }

                    if (playerManager.GetPlayerNetworkManager().isLockedOn.Value)
                    {
                        Vector3 relativeEnemyPosition = playerManager.transform.InverseTransformPoint(availableTargets[k].transform.position);
                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        if (availableTargets[k] == playerManager.GetPlayerCombatManager().currentTarget)
                        {
                            continue;
                        }

                        if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockOnTarget = availableTargets[k];
                        }
                        else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockOnTarget = availableTargets[k];
                        }
                    }
                }
                else
                {
                    ClearLockOnTargets();
                    playerManager.GetPlayerNetworkManager().isLockedOn.Value = false;
                }
            }
        }

        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;
            rightLockOnTarget = null;
            leftLockOnTarget = null;
            availableTargets.Clear();
        }

        public IEnumerator WaitThenFindNewTarget()
        {
            while (playerManager.GetIsPerformingAction())
            {
                yield return null;
            }

            ClearLockOnTargets();
            HandleLocatingLockOnTargets();

            if (nearestLockOnTarget != null)
            {
                playerManager.GetPlayerCombatManager().SetTarget(nearestLockOnTarget);
                playerManager.GetPlayerNetworkManager().isLockedOn.Value = true;
            }

            yield return null;
        }

        public void SetLockCameraHeight()
        {
            if (cameraLockOnHeightCoroutine != null)
            {
                StopCoroutine(cameraLockOnHeightCoroutine);
            }

            cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
        }

        private IEnumerator SetCameraHeight()
        {
            float duration = 1f;
            float timer = 0f;

            Vector3 velocity = Vector3.zero;
            Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
            Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

            while (timer < duration)
            {
                timer += Time.deltaTime;

                if (playerManager != null)
                {
                    if (playerManager.GetPlayerCombatManager().currentTarget != null)
                    {
                        cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                            cameraPivotTransform.transform.localPosition,
                            newLockedCameraHeight,
                            ref velocity,
                            setCameraHeightSpeed
                        );
                        cameraPivotTransform.transform.localRotation = Quaternion.Slerp(
                            cameraPivotTransform.transform.localRotation,
                            Quaternion.Euler(0, 0, 0),
                            lockOnTargetFollowSpeed
                        );
                    }
                    else
                    {
                        cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                            cameraPivotTransform.transform.localPosition,
                            newUnlockedCameraHeight,
                            ref velocity,
                            setCameraHeightSpeed
                        );
                    }
                }
                yield return null;
            }

            if (playerManager != null)
            {
                if (playerManager.GetPlayerCombatManager().currentTarget == null)
                {
                    cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                }
            }

            yield return null;
        }
    }
}
