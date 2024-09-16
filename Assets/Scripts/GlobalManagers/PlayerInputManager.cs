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

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

                // HERE WE HOLD THE DODGE BUTTON
                playerControls.PlayerActions.Dodge.performed += ctx => isDodgeInputActive = true;

                // HERE WE HOLD THE SPRINT BUTTON
                playerControls.PlayerActions.Sprint.performed += ctx => isSprintInputActive = true;
                playerControls.PlayerActions.Sprint.canceled += ctx => isSprintInputActive = false;

                // HERE WE HOLD THE JUMP BUTTON
                playerControls.PlayerActions.Jump.performed += ctx => isJumpInputActive = true;

                // HERE WE HANDLE ATTACK INPUTS
                playerControls.PlayerActions.Attack.performed += ctx => isAttacking = true;
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

        private void HandleAllInputs()
        {
            HandlePlayerMovementInput();
            HandleCameraMovemtInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleAttack();
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

            playerAnimationManager.UpdateAnimatorMovementParameters(0, playerMoveAmount, isSprinting);
        }

        private void HandleCameraMovemtInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
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

                playerManager.GetPlayerCombatManager().PerformWeaponBasedAction(playerManager.GetPlayerInventoryManager().currentRightHandWeapon.rightHandWeaponAction, playerManager.GetPlayerInventoryManager().currentRightHandWeapon);
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
