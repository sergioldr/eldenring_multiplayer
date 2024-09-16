using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        private PlayerManager playerManager;
        public WeaponItem currentWeaponBeingUsed;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
        }

        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            if (playerManager.IsOwner)
            {
                weaponAction.AttemptToPerformAction(playerManager, weaponPerformingAction);

                // TODO: NOTIFY THE SERVER WE HAVE PERFORMED AN ACTION
                ulong clientID = NetworkManager.Singleton.LocalClientId;
                playerManager.GetPlayerNetworkManager().NotifyTheServerOfWeaponActionServerRpc(clientID, weaponAction.actionID, weaponPerformingAction.itemID);
            }


        }
    }
}
