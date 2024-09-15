using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace SL
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        private PlayerManager playerManager;

        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Equipment")]
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

        public void OnCurrentRightHandWeaponIDChanged(int oldWeaponID, int newWeaponID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponItemByID(newWeaponID));
            playerManager.GetPlayerInventoryManager().currentRightHandWeapon = newWeapon;
            playerManager.GetPlayerEquipmentManager().LoadWeaponOnRightHand();
        }

        public void OnCurrentLeftHandWeaponIDChanged(int oldWeaponID, int newWeaponID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponItemByID(newWeaponID));
            playerManager.GetPlayerInventoryManager().currentLeftHandWeapon = newWeapon;
            playerManager.GetPlayerEquipmentManager().LoadWeaponOnLeftHand();
        }
    }
}
