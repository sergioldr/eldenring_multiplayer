using System.Linq;
using UnityEngine;

namespace SL
{
    public class WorldActionManager : MonoBehaviour
    {
        public static WorldActionManager Instance;

        [Header("Weapon Item Actions")]
        public WeaponItemAction[] weaponItemActions;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            AddAllActionsIds();
        }

        private void AddAllActionsIds()
        {
            for (int i = 0; i < weaponItemActions.Length; i++)
            {
                weaponItemActions[i].actionID = i;
            }
        }

        public WeaponItemAction GetWeaponItemActionByID(int actionID)
        {
            return weaponItemActions.FirstOrDefault(action => action.actionID == actionID);
        }
    }
}
