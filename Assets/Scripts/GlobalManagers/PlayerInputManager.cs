using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SL
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance { get; private set; }
        private PlayerControls playerControls;
        private PlayerManager playerManager;

        [Header("Camera Movement Inputs")]
        [SerializeField] private Vector2 cameraInput;
        [SerializeField] private float cameraVerticalInput;
        [SerializeField] private float cameraHorizontalInput;

        [Header("Lock On Inputs")]
        [SerializeField] private bool isLockOnInputActive = false;
        [SerializeField] private bool isLockOnRightInputActive = false;
        [SerializeField] private bool isLockOnLeftInputActive = false;
        private Coroutine lockOnCoroutine;

        [Header("Player Movement Inputs")]
        [SerializeField] private Vector2 movementInput;
        [SerializeField] private float playerVerticalInput;
        [SerializeField] private float playerHorizontalInput;
        [SerializeField] private float playerMoveAmount;

        [Header("Player Action Inputs")]
        [SerializeField] private bool isDodgeInputActive = false;
        [SerializeField] private bool isSprintInputActive = false;
        [SerializeField] private bool isJumpInputActive = false;
        [SerializeField] private bool isAttacking = false;
        [SerializeField] private bool isTriggerHeavyAttackInput = false;
        [SerializeField] private bool isHoldingHeavyAttackInput = false;
        [SerializeField] private bool isSwitchingRightHandWeaponInput = false;
        [SerializeField] private bool isSwitchingLeftHandWeaponInput = false;




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

            SceneManager.activeSceneChanged += SceneManager_OnSceneChanged;

            Instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

                // HERE WE HOLD THE LOCK ON BUTTON
                playerControls.PlayerActions.LockOn.performed += ctx => isLockOnInputActive = true;
                playerControls.PlayerActions.SeekRightLockOnTarget.performed += ctx => isLockOnRightInputActive = true;
                playerControls.PlayerActions.SeekLeftLockOnTarget.performed += ctx => isLockOnLeftInputActive = true;

                // HERE WE HOLD THE DODGE BUTTON
                playerControls.PlayerActions.Dodge.performed += ctx => isDodgeInputActive = true;

                // HERE WE HOLD THE SPRINT BUTTON
                playerControls.PlayerActions.Sprint.performed += ctx => isSprintInputActive = true;
                playerControls.PlayerActions.Sprint.canceled += ctx => isSprintInputActive = false;

                // HERE WE HOLD THE JUMP BUTTON
                playerControls.PlayerActions.Jump.performed += ctx => isJumpInputActive = true;

                // HERE WE HANDLE ATTACK INPUTS
                playerControls.PlayerActions.Attack.performed += ctx => isAttacking = true;

                // HERE WE HANDLE HEAVY ATTACK INPUTS
                playerControls.PlayerActions.HeavyAttack.performed += ctx => isTriggerHeavyAttackInput = true;
                playerControls.PlayerActions.ChargeHeavyAttack.performed += ctx => isHoldingHeavyAttackInput = true;
                playerControls.PlayerActions.ChargeHeavyAttack.canceled += ctx => isHoldingHeavyAttackInput = false;

                // HERE WE HANDLE SWITCHING WEAPONS
                playerControls.PlayerActions.SwitchRightWeapon.performed += ctx => isSwitchingRightHandWeaponInput = true;
                playerControls.PlayerActions.SwitchLeftWeapon.performed += ctx => isSwitchingLeftHandWeaponInput = true;
            }

            playerControls.Enable();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_OnSceneChanged;
        }

        private void SceneManager_OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())
            {
                Instance.enabled = true;

                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            else
            {
                Instance.enabled = false;

                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovemtInput();
            HandleLockOnInput();
            HandleLockOnSwitchTargetInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleAttack();
            HandleHeavyAttacks();
            HandleChargeHeavyAttack();
            HandleSwitchRightWeapon();
            HandleSwitchLeftWeapon();
        }

        private void HandlePlayerMovementInput()
        {
            playerVerticalInput = movementInput.y;
            playerHorizontalInput = movementInput.x;

            playerMoveAmount = Mathf.Clamp01(Mathf.Abs(playerHorizontalInput) + Mathf.Abs(playerVerticalInput));

            if (playerMoveAmount <= 0.5 && playerMoveAmount > 0)
            {
                playerMoveAmount = 0.5f;
            }
            else if (playerMoveAmount > 0.5 && playerMoveAmount <= 1)
            {
                playerMoveAmount = 1;
            }


            PlayerAnimationManager playerAnimationManager = playerManager.GetPlayerAnimationManager();
            bool isSprinting = playerManager.GetPlayerNetworkManager().isSprinting.Value;
            bool isLockedOn = playerManager.GetPlayerNetworkManager().isLockedOn.Value;

            if (!isLockedOn || isSprinting)
            {
                playerAnimationManager.UpdateAnimatorMovementParameters(0, playerMoveAmount, isSprinting);
            }
            else
            {
                playerAnimationManager.UpdateAnimatorMovementParameters(playerHorizontalInput, playerVerticalInput, isSprinting);
            }

        }

        private void HandleCameraMovemtInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
        }

        private void HandleLockOnInput()
        {
            if (playerManager.GetPlayerNetworkManager().isLockedOn.Value)
            {
                if (playerManager.GetPlayerCombatManager().currentTarget == null)
                {
                    return;
                }

                if (playerManager.GetPlayerCombatManager().currentTarget.GetIsDead())
                {
                    playerManager.GetPlayerNetworkManager().isLockedOn.Value = false;
                }

                if (lockOnCoroutine != null)
                {
                    StopCoroutine(lockOnCoroutine);
                }

                lockOnCoroutine = StartCoroutine(PlayerCamera.Instance.WaitThenFindNewTarget());
            }
            if (isLockOnInputActive && playerManager.GetPlayerNetworkManager().isLockedOn.Value)
            {
                isLockOnInputActive = false;
                PlayerCamera.Instance.ClearLockOnTargets();
                playerManager.GetPlayerNetworkManager().isLockedOn.Value = false;
                return;
            }
            if (isLockOnInputActive && !playerManager.GetPlayerNetworkManager().isLockedOn.Value)
            {
                isLockOnInputActive = false;

                PlayerCamera.Instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.Instance.nearestLockOnTarget != null)
                {
                    playerManager.GetPlayerCombatManager().SetTarget(PlayerCamera.Instance.nearestLockOnTarget);
                    playerManager.GetPlayerNetworkManager().isLockedOn.Value = true;
                }
            }
        }

        private void HandleLockOnSwitchTargetInput()
        {
            if (isLockOnLeftInputActive)
            {
                isLockOnLeftInputActive = false;

                if (playerManager.GetPlayerNetworkManager().isLockedOn.Value)
                {
                    PlayerCamera.Instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.Instance.leftLockOnTarget != null)
                    {
                        playerManager.GetPlayerCombatManager().SetTarget(PlayerCamera.Instance.leftLockOnTarget);
                    }
                }
            }

            if (isLockOnRightInputActive)
            {
                isLockOnRightInputActive = false;

                if (playerManager.GetPlayerNetworkManager().isLockedOn.Value)
                {
                    PlayerCamera.Instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.Instance.rightLockOnTarget != null)
                    {
                        playerManager.GetPlayerCombatManager().SetTarget(PlayerCamera.Instance.rightLockOnTarget);
                    }
                }
            }
        }

        private void HandleDodgeInput()
        {
            if (isDodgeInputActive)
            {
                isDodgeInputActive = false;
                playerManager.GetPlayerLocomotionManager().AttemptToDodge();
            }
        }

        private void HandleSprintInput()
        {
            if (isSprintInputActive)
            {
                playerManager.GetPlayerLocomotionManager().HandleSprinting();
            }
            else
            {
                playerManager.GetPlayerNetworkManager().isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (isJumpInputActive)
            {
                isJumpInputActive = false;
                playerManager.GetPlayerLocomotionManager().AttemptToJump();
            }
        }

        private void HandleAttack()
        {
            if (isAttacking)
            {
                isAttacking = false;

                //TODO: IF WE HAVE A UI WINDOW OPEN, WE SHOULD NOT BE ABLE TO ATTACK

                playerManager.GetPlayerNetworkManager().SetCharacterActionHand(true);

                // TODO: HANDLE DOUBLE WEAPON ATTACKS

                PlayerCombatManager playerCombatManager = playerManager.GetPlayerCombatManager();
                PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

                playerCombatManager.PerformWeaponBasedAction(playerInventoryManager.currentRightHandWeapon.rightHandWeaponAction, playerInventoryManager.currentRightHandWeapon);
            }
        }

        private void HandleHeavyAttacks()
        {
            if (isTriggerHeavyAttackInput)
            {
                isTriggerHeavyAttackInput = false;

                playerManager.GetPlayerNetworkManager().SetCharacterActionHand(true);

                PlayerCombatManager playerCombatManager = playerManager.GetPlayerCombatManager();
                PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

                playerCombatManager.PerformWeaponBasedAction(playerInventoryManager.currentRightHandWeapon.rightHandHeavyWeaponAction, playerInventoryManager.currentRightHandWeapon);
            }
        }

        private void HandleChargeHeavyAttack()
        {
            if (playerManager.GetIsPerformingAction())
            {
                if (playerManager.GetPlayerNetworkManager().isUsingRightHandWeapon.Value)
                {
                    playerManager.GetPlayerNetworkManager().isChargingHeavyAttack.Value = isHoldingHeavyAttackInput;
                }
            }
        }

        private void HandleSwitchRightWeapon()
        {
            if (isSwitchingRightHandWeaponInput)
            {
                isSwitchingRightHandWeaponInput = false;
                playerManager.GetPlayerEquipmentManager().SwitchRightWeapon();
            }
        }

        private void HandleSwitchLeftWeapon()
        {
            if (isSwitchingLeftHandWeaponInput)
            {
                isSwitchingLeftHandWeaponInput = false;
                playerManager.GetPlayerEquipmentManager().SwitchLeftWeapon();
            }
        }

        // GETTERS AND SETTERS

        public float GetMovementAmount()
        {
            return playerMoveAmount;
        }

        public float GetVerticalMovement()
        {
            return playerVerticalInput;
        }

        public float GetHorizontalMovement()
        {
            return playerHorizontalInput;
        }

        public float GetCameraVerticalInput()
        {
            return cameraVerticalInput;
        }

        public float GetCameraHorizontalInput()
        {
            return cameraHorizontalInput;
        }

        public void SetPlayerManager(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        public bool GetIsSprintInputActive()
        {
            return isSprintInputActive;
        }

        public void SetIsSprintInputActive(bool isSprintInputActive)
        {
            this.isSprintInputActive = isSprintInputActive;
        }
    }
}
