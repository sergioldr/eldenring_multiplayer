using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace SL
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        private PlayerManager playerManager;

        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        protected override void Awake()
        {
            base.Awake();
            playerManager = GetComponent<PlayerManager>();
        }

        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            networkMaxHealth.Value = playerManager.GetPlayerStatsManager().CalculateBaseHealthBasedOnVitalityLevel(newVitality);
            PlayerUIManager.Instance.GetPlayerUIHUDManager().SetMaxHealthValue(networkMaxHealth.Value);
            networkCurrentHealth.Value = networkMaxHealth.Value;
        }

        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            networkMaxStamina.Value = playerManager.GetPlayerStatsManager().CalculateBaseStaminaBasedOnEnduranceLevel(newEndurance);
            PlayerUIManager.Instance.GetPlayerUIHUDManager().SetMaxStaminaValue(networkMaxStamina.Value);
            networkCurrentStamina.Value = networkMaxStamina.Value;
        }
    }
}
