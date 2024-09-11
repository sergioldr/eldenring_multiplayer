using UnityEngine;

namespace SL
{
    public class PlayerManager : CharacterManager
    {
        private PlayerLocomotionManager playerLocomotionManager;
        private PlayerAnimationManager playerAnimationManager;
        private PlayerNetworkManager playerNetworkManager;
        private PlayerStatsManager playerStatsManager;

        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimationManager = GetComponent<PlayerAnimationManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (!IsOwner) return;

            playerLocomotionManager.HandleAllMovement();
            playerStatsManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!IsOwner) return;

            PlayerCamera.Instance.HandleAllCameraActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PlayerCamera.Instance.SetPlayerManager(this);
                PlayerInputManager.Instance.SetPlayerManager(this);

                PlayerUIHUDManager playerUIHUDManager = PlayerUIManager.Instance.GetPlayerUIHUDManager();

                playerNetworkManager.networkCurrentStamina.OnValueChanged += playerUIHUDManager.SetNewStaminaValue;
                playerNetworkManager.networkCurrentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenerationTimer;

                playerNetworkManager.networkMaxStamina.Value = playerStatsManager.CalculateBaseStaminaBasedOnEnduranceLevel(playerNetworkManager.networkEndurance.Value);
                playerNetworkManager.networkCurrentStamina.Value = playerStatsManager.CalculateBaseStaminaBasedOnEnduranceLevel(playerNetworkManager.networkEndurance.Value);
                playerUIHUDManager.SetMaxStaminaValue(playerNetworkManager.networkMaxStamina.Value);
            }
        }

        public PlayerAnimationManager GetPlayerAnimationManager()
        {
            return playerAnimationManager;
        }

        public PlayerLocomotionManager GetPlayerLocomotionManager()
        {
            return playerLocomotionManager;
        }

        public PlayerNetworkManager GetPlayerNetworkManager()
        {
            return playerNetworkManager;
        }
    }
}
