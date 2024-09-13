using UnityEngine;

namespace SL
{
    public class CharacterStatsManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        [Header("Character Stats")]
        [SerializeField] private float staminaRegenerationAmount = 2f;
        [SerializeField] private float staminaRegenerationDelay = 2f;
        private float staminaRegenerationTimer = 0f;
        private float staminaTickTimer = 0f;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }


        protected virtual void Start()
        {

        }

        public int CalculateBaseHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }

        public int CalculateBaseStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public virtual void RegenerateStamina()
        {
            if (!characterManager.IsOwner) return;

            CharacterNetworkManager characterNetworkManager = characterManager.GetCharacterNetworkManager();

            if (characterNetworkManager.isSprinting.Value) return;
            if (characterManager.GetIsPerformingAction()) return;

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (characterNetworkManager.networkCurrentStamina.Value < characterNetworkManager.networkMaxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1f)
                    {
                        staminaTickTimer = 0f;
                        characterNetworkManager.networkCurrentStamina.Value += staminaRegenerationAmount;
                    }
                };
            }
        }

        public virtual void ResetStaminaRegenerationTimer(float previousStaminaValue, float currentStaminaAmount)
        {
            if (currentStaminaAmount < previousStaminaValue)
            {
                staminaRegenerationTimer = 0f;
            }
        }
    }
}
