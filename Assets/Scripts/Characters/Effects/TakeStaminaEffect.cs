using UnityEngine;

namespace SL
{
    [CreateAssetMenu(menuName = "Character/Effects/Take Stamina")]
    public class TakeStaminaEffect : InstantCharacterEffect
    {
        [SerializeField] private float staminaDamageAmount = 10;

        public override void ProcessEffect(CharacterManager characterManager)
        {
            CalculateStaminaDamage(characterManager);
        }

        private void CalculateStaminaDamage(CharacterManager characterManager)
        {
            if (characterManager.IsOwner)
            {
                CharacterNetworkManager characterNetworkManager = characterManager.GetCharacterNetworkManager();
                characterNetworkManager.networkCurrentStamina.Value -= staminaDamageAmount;
            }
        }
    }
}
