using UnityEngine;

namespace SL
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager playerManager;

        private float verticalMovement;
        private float horizontalMovement;
        private float moveAmount;

        [Header("Movement Settings")]
        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] private float walkingSpeed = 2.5f;
        [SerializeField] private float runningSpeed = 5f;
        [SerializeField] private float rotationSpeed = 15f;

        [Header("Dodge Settings")]
        private Vector3 rollDirection;

        protected override void Awake()
        {
            base.Awake();
            playerManager = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            CharacterNetworkManager characterNetworkManager = playerManager.GetCharacterNetworkManager();
            PlayerAnimationManager playerAnimationManager = playerManager.GetPlayerAnimationManager();

            if (playerManager.IsOwner)
            {
                characterNetworkManager.networkVerticalMovement.Value = verticalMovement;
                characterNetworkManager.networkHorizontalMovement.Value = horizontalMovement;
                characterNetworkManager.networkAmountMovement.Value = moveAmount;
            }
            else
            {
                verticalMovement = characterNetworkManager.networkVerticalMovement.Value;
                horizontalMovement = characterNetworkManager.networkHorizontalMovement.Value;
                moveAmount = characterNetworkManager.networkAmountMovement.Value;

                playerAnimationManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement);
            }
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.Instance.GetVerticalMovement();
            horizontalMovement = PlayerInputManager.Instance.GetHorizontalMovement();
            moveAmount = PlayerInputManager.Instance.GetMovementAmount();
        }

        private void HandleGroundedMovement()
        {
            if (!playerManager.GetCanMove()) return;

            GetMovementValues();

            moveDirection = PlayerCamera.Instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.Instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float movementAmount = PlayerInputManager.Instance.GetMovementAmount();
            CharacterController characterController = playerManager.GetCharacterController();

            if (movementAmount > 0.5f)
            {
                characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (movementAmount <= 0.5f)
            {
                characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if (!playerManager.GetCanRotate()) return;

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

        public void AttemptToDodge()
        {
            bool isPerformingAction = playerManager.GetIsPerformingAction();

            if (isPerformingAction) return;

            PlayerAnimationManager playerAnimationManager = playerManager.GetPlayerAnimationManager();
            // If we are moving when we press the dodge button we perfrom a roll
            if (PlayerInputManager.Instance.GetMovementAmount() > 0)
            {
                Transform cameraTransform = PlayerCamera.Instance.GetCamera().transform;

                rollDirection = cameraTransform.forward * PlayerInputManager.Instance.GetVerticalMovement();
                rollDirection += cameraTransform.right * PlayerInputManager.Instance.GetHorizontalMovement();
                rollDirection.y = 0;
                rollDirection.Normalize();

                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                playerManager.transform.rotation = playerRotation;

                playerAnimationManager.PlayTargetActionAnimation("Roll_Forward_01", true);
            }
            else
            {
                // If we are not moving when we press the dodge button we perform a backstep
                playerAnimationManager.PlayTargetActionAnimation("Back_Step_01", true);
            }
        }

        public void HandleSprinting()
        {

        }
    }
}

