using UnityEngine;

namespace SL
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string light_attack_01 = "Main_Light_Attack_01";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponItemAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponItemAction);

            if (!playerPerformingAction.IsOwner) return;

            // CHECK FOR STOPPERS
            PlayerNetworkManager playerNetworkManager = playerPerformingAction.GetPlayerNetworkManager();

            if (playerNetworkManager.networkCurrentStamina.Value <= 0) return;
            if (!playerPerformingAction.GetIsGrounded()) return;

            PerformLightAttack(playerPerformingAction, weaponItemAction);
        }

        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponItemAction)
        {
            PlayerNetworkManager playerNetworkManager = playerPerformingAction.GetPlayerNetworkManager();
            PlayerAnimationManager playerAnimationManager = playerPerformingAction.GetPlayerAnimationManager();

            if (playerNetworkManager.isUsingRightHandWeapon.Value)
            {
                playerAnimationManager.PlayTargetAttackActionAnimation(light_attack_01, true);
            }

            if (playerNetworkManager.isUsingLeftHandWeapon.Value)
            {

            }
        }
    }
}
