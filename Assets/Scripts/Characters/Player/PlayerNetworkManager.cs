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
        public NetworkVariable<int> currentWeaponBeingUsedID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingRightHandWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingLeftHandWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

        public void OnCurrentWeaponBeingUsedIDChanged(int oldWeaponID, int newWeaponID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponItemByID(newWeaponID));
            playerManager.GetPlayerCombatManager().currentWeaponBeingUsed = newWeapon;
        }

        public void SetCharacterActionHand(bool rightHandWeaponAction)
        {
            if (rightHandWeaponAction)
            {
                isUsingRightHandWeapon.Value = true;
                isUsingLeftHandWeapon.Value = false;
            }
            else
            {
                isUsingRightHandWeapon.Value = false;
                isUsingLeftHandWeapon.Value = true;
            }
        }

        [ServerRpc]
        public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
        {
            if (IsServer)
            {
                NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
            }
        }

        [ClientRpc]
        private void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerfomWeaponBasedAction(actionID, weaponID);
            }
        }

        private void PerfomWeaponBasedAction(int actionID, int weaponID)
        {
            WeaponItemAction weaponItemAction = WorldActionManager.Instance.GetWeaponItemActionByID(actionID);

            if (weaponItemAction != null)
            {
                weaponItemAction.AttemptToPerformAction(playerManager, WorldItemDatabase.Instance.GetWeaponItemByID(weaponID));
            }
            else
            {
                Debug.LogError("Weapon Item Action is null");
            }
        }
    }
}
