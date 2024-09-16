using System;
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

        public virtual void DrainStaminaBasedOnAttack()
        {
            if (!playerManager.IsOwner) return;
            if (currentWeaponBeingUsed == null) return;

            float staminaDeducted = 0;

            switch (currentAttackType)
            {
                case AttackType.LightAttack:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackModifier;
                    break;
                default:
                    break;
            }

            playerManager.GetPlayerNetworkManager().networkCurrentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }
    }
}
