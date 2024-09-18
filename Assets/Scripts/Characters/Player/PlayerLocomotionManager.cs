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
        [SerializeField] private float sprintingSpeed = 7f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private float sprintingStaminaCost = 2f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 4f;
        [SerializeField] private float jumpStaminaCost = 25f;
        [SerializeField] private float jumpForwardSpeed = 5f;
        [SerializeField] private float jumpFreeFallSpeed = 2f;
        private Vector3 jumpDirection;

        [Header("Dodge Settings")]
        private Vector3 rollDirection;
        [SerializeField] private float dodgeStaminaCost = 25f;

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

                bool isSprinting = characterNetworkManager.isSprinting.Value;
                bool isLockedOn = playerManager.GetPlayerNetworkManager().isLockedOn.Value;

                if (!isLockedOn || isSprinting)
                {
                    playerAnimationManager.UpdateAnimatorMovementParameters(0, moveAmount, isSprinting);
                }
                else
                {
                    playerAnimationManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, isSprinting);
                }

            }
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();
            HandleJumpingMovement();
            HandleFreeFallMovement();
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

            if (playerManager.GetPlayerNetworkManager().isSprinting.Value)
            {
                characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
                if (movementAmount > 0.5f)
                {
                    characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (movementAmount <= 0.5f)
                {
                    characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }
        }

        private void HandleJumpingMovement()
        {
            if (playerManager.GetPlayerNetworkManager().isJumping.Value)
            {
                playerManager.GetCharacterController().Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }

        private void HandleFreeFallMovement()
        {
            if (!playerManager.GetIsGrounded())
            {
                Vector3 freeFallDirection = PlayerCamera.Instance.GetCamera().transform.forward * PlayerInputManager.Instance.GetVerticalMovement();
                freeFallDirection += PlayerCamera.Instance.GetCamera().transform.right * PlayerInputManager.Instance.GetHorizontalMovement();
                freeFallDirection.y = 0;

                playerManager.GetCharacterController().Move(freeFallDirection * jumpFreeFallSpeed * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if (playerManager.GetIsDead()) return;
            if (!playerManager.GetCanRotate()) return;

            if (playerManager.GetPlayerNetworkManager().isLockedOn.Value)
            {
                if (playerManager.GetPlayerNetworkManager().isSprinting.Value || playerManager.GetPlayerLocomotionManager().isRolling)
                {
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = PlayerCamera.Instance.GetCamera().transform.forward * verticalMovement;
                    targetDirection += PlayerCamera.Instance.GetCamera().transform.right * horizontalMovement;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                    {
                        targetDirection = transform.forward;
                    }

                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRotation;
                }
                else
                {
                    if (playerManager.GetPlayerCombatManager().currentTarget == null) return;

                    Vector3 targetDirection;
                    targetDirection = playerManager.GetPlayerCombatManager().currentTarget.transform.position - transform.position;
                    targetDirection.Normalize();
                    targetDirection.y = 0;
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRotation;
                }
            }
            else
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

        public void AttemptToDodge()
        {
            bool isPerformingAction = playerManager.GetIsPerformingAction();
            if (isPerformingAction) return;

            PlayerNetworkManager playerNetworkManager = playerManager.GetPlayerNetworkManager();

            if (playerNetworkManager.networkCurrentStamina.Value <= 0) return;

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
                playerManager.GetPlayerLocomotionManager().isRolling = true;
            }
            else
            {
                // If we are not moving when we press the dodge button we perform a backstep
                playerAnimationManager.PlayTargetActionAnimation("Back_Step_01", true);
            }

            playerNetworkManager.networkCurrentStamina.Value -= dodgeStaminaCost;
        }

        public void HandleSprinting()
        {
            PlayerNetworkManager playerNetworkManager = playerManager.GetPlayerNetworkManager();

            if (playerManager.GetIsPerformingAction())
            {
                playerNetworkManager.isSprinting.Value = false;
            }

            if (playerNetworkManager.networkCurrentStamina.Value <= 0)
            {
                playerNetworkManager.isSprinting.Value = false;
                return;
            }

            if (moveAmount >= 0.5f)
            {
                playerNetworkManager.isSprinting.Value = true;
            }
            else
            {
                playerNetworkManager.isSprinting.Value = false;
            }

            if (playerNetworkManager.isSprinting.Value)
            {
                playerNetworkManager.networkCurrentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }
        }

        public void AttemptToJump()
        {
            bool isPerformingAction = playerManager.GetIsPerformingAction();
            if (isPerformingAction) return;

            PlayerNetworkManager playerNetworkManager = playerManager.GetPlayerNetworkManager();

            if (playerNetworkManager.networkCurrentStamina.Value <= 0) return;

            PlayerAnimationManager playerAnimationManager = playerManager.GetPlayerAnimationManager();

            if (playerNetworkManager.isJumping.Value) return;

            if (!playerManager.GetIsGrounded()) return;

            playerAnimationManager.PlayTargetActionAnimation("Main_Jump_01", false);

            playerNetworkManager.isJumping.Value = true;

            playerNetworkManager.networkCurrentStamina.Value -= jumpStaminaCost;

            jumpDirection = PlayerCamera.Instance.GetCamera().transform.forward * PlayerInputManager.Instance.GetVerticalMovement();
            jumpDirection += PlayerCamera.Instance.GetCamera().transform.right * PlayerInputManager.Instance.GetHorizontalMovement();
            jumpDirection.y = 0;

            if (jumpDirection != Vector3.zero)
            {
                if (playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1;
                }
                else if (PlayerInputManager.Instance.GetMovementAmount() > 0.5f)
                {
                    jumpDirection *= 0.5f;
                }
                else if (PlayerInputManager.Instance.GetMovementAmount() <= 0.5f)
                {
                    jumpDirection *= 0.25f;
                }
            }
        }

        public void ApplyJumpingVelocity()
        {
            jumpForceVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }
    }
}

