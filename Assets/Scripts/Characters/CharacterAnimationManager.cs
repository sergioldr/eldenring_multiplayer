using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        CharacterManager characterManager;

        private int horizontal;
        private int vertical;

        [Header("Damage Animations")]
        public string lastDamageAnimationPlayed;

        [SerializeField] private string hit_forward_medium_01 = "hit_forward_medium_01";
        [SerializeField] private string hit_forward_medium_02 = "hit_forward_medium_02";

        [SerializeField] private string hit_backward_medium_01 = "hit_backward_medium_01";
        [SerializeField] private string hit_backward_medium_02 = "hit_backward_medium_02";

        [SerializeField] private string hit_left_medium_01 = "hit_left_medium_01";
        [SerializeField] private string hit_left_medium_02 = "hit_left_medium_02";

        [SerializeField] private string hit_right_medium_01 = "hit_right_medium_01";
        [SerializeField] private string hit_right_medium_02 = "hit_right_medium_02";

        public List<string> forward_medium_damage { get; private set; } = new List<string>();
        public List<string> backward_medium_damage { get; private set; } = new List<string>();
        public List<string> left_medium_damage { get; private set; } = new List<string>();
        public List<string> right_medium_damage { get; private set; } = new List<string>();

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        protected virtual void Start()
        {
            forward_medium_damage.Add(hit_forward_medium_01);
            forward_medium_damage.Add(hit_forward_medium_02);

            backward_medium_damage.Add(hit_backward_medium_01);
            backward_medium_damage.Add(hit_backward_medium_02);

            left_medium_damage.Add(hit_left_medium_01);
            left_medium_damage.Add(hit_left_medium_02);

            right_medium_damage.Add(hit_right_medium_01);
            right_medium_damage.Add(hit_right_medium_02);
        }

        public string GetRandomAnimation(List<string> animationList)
        {
            List<string> availableAnimations = new List<string>();

            foreach (string animation in animationList)
            {
                availableAnimations.Add(animation);
            }

            availableAnimations.Remove(lastDamageAnimationPlayed);

            for (int i = availableAnimations.Count - 1; i > 0; i--)
            {
                if (availableAnimations[i] == null)
                {
                    availableAnimations.RemoveAt(i);
                }
            }

            return availableAnimations[Random.Range(0, availableAnimations.Count)];
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
            Debug.Log("Playing Animation: " + targetAnimation);

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

