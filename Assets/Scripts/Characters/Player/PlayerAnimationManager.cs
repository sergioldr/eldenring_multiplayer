using UnityEngine;

namespace SL
{
    public class PlayerAnimationManager : CharacterAnimationManager
    {
        private PlayerManager playerManager;

        protected override void Awake()
        {
            base.Awake();
            playerManager = GetComponent<PlayerManager>();
        }
        private void OnAnimatorMove()
        {
            if (playerManager.GetApplyRootMotion())
            {
                Animator playerAnimator = playerManager.GetCharacterAnimator();
                Vector3 velocity = playerAnimator.deltaPosition;
                playerManager.GetCharacterController().Move(velocity);
                playerManager.transform.rotation *= playerAnimator.deltaRotation;
            }
        }
    }
}