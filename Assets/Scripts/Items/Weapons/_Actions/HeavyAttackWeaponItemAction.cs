using UnityEngine;

namespace SL
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string heavy_attack_01 = "Main_Heavy_Attack_01";

        public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponItemAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponItemAction);

            if (!playerPerformingAction.IsOwner) return;

            // CHECK FOR STOPPERS
            PlayerNetworkManager playerNetworkManager = playerPerformingAction.GetPlayerNetworkManager();

            if (playerNetworkManager.networkCurrentStamina.Value <= 0) return;
            if (!playerPerformingAction.GetIsGrounded()) return;

            PerformHeavyAttack(playerPerformingAction, weaponItemAction);
        }

        private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponItemAction)
        {
            PlayerNetworkManager playerNetworkManager = playerPerformingAction.GetPlayerNetworkManager();
            PlayerAnimationManager playerAnimationManager = playerPerformingAction.GetPlayerAnimationManager();

            if (playerNetworkManager.isUsingRightHandWeapon.Value)
            {
                playerAnimationManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack, heavy_attack_01, true);
            }

            if (playerNetworkManager.isUsingLeftHandWeapon.Value)
            {

            }
        }
    }
}
