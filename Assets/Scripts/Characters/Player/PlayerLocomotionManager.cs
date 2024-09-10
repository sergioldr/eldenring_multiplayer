using UnityEngine;

namespace SL
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager playerManager;
        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;

        [SerializeField] private float walkingSpeed = 2.5f;
        [SerializeField] private float runningSpeed = 5f;
        [SerializeField] private float rotationSpeed = 15f;

        protected override void Awake()
        {
            base.Awake();
            playerManager = GetComponent<PlayerManager>();
        }


        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
        }

        private void GetVerticalAndHorizontalInput()
        {
            verticalMovement = PlayerInputManager.Instance.GetVerticalMovement();
            horizontalMovement = PlayerInputManager.Instance.GetHorizontalMovement();
        }

        private void HandleGroundedMovement()
        {
            GetVerticalAndHorizontalInput();

            moveDirection = PlayerCamera.Instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.Instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float movementAmount = PlayerInputManager.Instance.GetMoveAmount();

            if (movementAmount > 0.5f)
            {
                playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (movementAmount <= 0.5f)
            {
                playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            targetRotationDirection = Vector3.zero;
            Camera playerCamera = PlayerCamera.Instance.GetCamera();
            targetRotationDirection = playerCamera.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + playerCamera.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }
}

