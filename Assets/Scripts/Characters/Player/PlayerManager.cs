using UnityEngine;

namespace SL
{
    public class PlayerManager : CharacterManager
    {
        private PlayerLocomotionManager playerLocomotionManager;
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
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
            }
        }
    }
}
