using UnityEngine;

namespace SL
{
    [CreateAssetMenu(menuName = "Character/Effects/Take Stamina Damage")]
    public class TakeStaminaDamageCharacterEffect : InstantCharacterEffect
    {
        [SerializeField] private float staminaDamageAmount = 10;

        public override void ProcessEffect(CharacterManager characterManager)
        {
            CalculateStaminaDamage(characterManager);
        }

        private void CalculateStaminaDamage(CharacterManager characterManager)
        {
            Debug.Log("TakeStaminaDamageCharacterEffect: Processing Instant Effect");

            if (characterManager.IsOwner)
            {
                Debug.Log("TakeStaminaDamageCharacterEffect: Processing Instant Effect on Owner");
                CharacterNetworkManager characterNetworkManager = characterManager.GetCharacterNetworkManager();
                characterNetworkManager.networkCurrentStamina.Value -= staminaDamageAmount;
            }
        }
    }
}
