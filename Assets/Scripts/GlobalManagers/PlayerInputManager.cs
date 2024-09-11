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
        PlayerControls playerControls;

        [Header("Player Movement Inputs")]
        [SerializeField] Vector2 movementInput;
        [SerializeField] float playerVerticalInput;
        [SerializeField] float playerHorizontalInput;
        [SerializeField] float playerMoveAmount;

        [Header("Camera Movement Inputs")]
        [SerializeField] Vector2 cameraInput;
        [SerializeField] float cameraVerticalInput;
        [SerializeField] float cameraHorizontalInput;

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
        }

        private void Update()
        {
            HandlePlayerMovementInput();
            HandleCameraMovemtInput();
        }

        private void SceneManager_OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.buildIndex == WorldSaveGameManager.Instance.GetWorldSceneIndex())
            {
                Instance.enabled = true;
            }
            else
            {
                Instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_OnSceneChanged;
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
        }

        private void HandleCameraMovemtInput()
        {
            cameraVerticalInput = cameraInput.y;
            cameraHorizontalInput = cameraInput.x;
            Debug.Log(cameraVerticalInput);
            Debug.Log(cameraHorizontalInput);
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

        public float GetMoveAmount()
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
    }
}
