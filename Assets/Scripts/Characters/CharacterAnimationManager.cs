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
    }
}

