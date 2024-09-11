using UnityEngine;

namespace SL
{
    public class PlayerManager : CharacterManager
    {
        private PlayerLocomotionManager playerLocomotionManager;
        private PlayerAnimationManager playerAnimationManager;
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimationManager = GetComponent<PlayerAnimationManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (!IsOwner) return;

            playerLocomotionManager.HandleAllMovement();
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
    }
}
