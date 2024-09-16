using UnityEngine;

namespace SL
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
    public class WeaponItemAction : ScriptableObject
    {
        public int actionID;

        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponItemAction)
        {
            if (playerPerformingAction.IsOwner)
            {
                playerPerformingAction.GetPlayerNetworkManager().currentWeaponBeingUsedID.Value = weaponItemAction.itemID;
            }

            Debug.Log("Performing action");
        }
    }
}
