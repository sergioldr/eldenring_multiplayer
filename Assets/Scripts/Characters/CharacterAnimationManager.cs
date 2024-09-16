using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        CharacterManager characterManager;

        private int horizontal;
        private int vertical;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
        {
            Animator characterAnimator = characterManager.GetCharacterAnimator();

            float horizontalAmount = horizontalValue;
            float verticalAmount = verticalValue;

            if (isSprinting)
            {
                verticalAmount = 2;
            }

            characterAnimator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            characterAnimator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
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
            characterManager.SetApplyRootMotion(applyRootMotion);
            characterAnimator.CrossFade(targetAnimation, 0.2f);

            // This is to stop character performing other actions for example if you are stunned you can't perform any other action
            characterManager.SetIsPerformingAction(isPerformingAction);
            characterManager.SetApplyRootMotion(applyRootMotion);
            characterManager.SetCanRotate(canRotate);
            characterManager.SetCanMove(canMove);

            // Tell the SERVER/HOST we played an animation and play that animation for everybody else in the game
            characterManager.GetCharacterNetworkManager().NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public virtual void PlayTargetAttackActionAnimation(
            AttackType attackType,
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false
        )
        {
            Animator characterAnimator = characterManager.GetCharacterAnimator();
            CharacterCombatManager characterCombatManager = characterManager.GetCharacterCombatManager();

            characterCombatManager.currentAttackType = attackType;

            characterManager.SetApplyRootMotion(applyRootMotion);
            characterAnimator.CrossFade(targetAnimation, 0.2f);

            // This is to stop character performing other actions for example if you are stunned you can't perform any other action
            characterManager.SetIsPerformingAction(isPerformingAction);
            characterManager.SetCanRotate(canRotate);
            characterManager.SetCanMove(canMove);

            // Tell the SERVER/HOST we played an animation and play that animation for everybody else in the game
            characterManager.GetCharacterNetworkManager().NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }
    }
}

