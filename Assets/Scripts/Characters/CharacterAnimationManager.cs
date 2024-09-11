using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        CharacterManager characterManager;

        private float horizontalValue;
        private float verticalValue;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }
        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
        {
            Animator characterAnimator = characterManager.GetCharacterAnimator();
            characterAnimator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
            characterAnimator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false
        )
        {
            Animator characterAnimator = characterManager.GetCharacterAnimator();
            //characterAnimator.applyRootMotion = applyRootMotion;
            characterAnimator.CrossFade(targetAnimation, 0.2f);

            // This is to stop character performing other actions for example if you are stunned you can't perform any other action
            characterManager.SetIsPerformingAction(isPerformingAction);
            characterManager.SetApplyRootMotion(applyRootMotion);
            characterManager.SetCanRotate(canRotate);
            characterManager.SetCanMove(canMove);

            // Tell the SERVER/HOST we played an animation and play that animation for everybody else in the game
            characterManager.GetCharacterNetworkManager().NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
    }
}

