using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SL
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase Instance { get; private set; }

        private List<Item> items = new List<Item>();

        [Header("Weapons")]
        [SerializeField] public WeaponItem unarmedWeapon;
        [SerializeField] private List<WeaponItem> weapons = new List<WeaponItem>();

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            AddAllWeaponsToItemList();
            AssignItemsIds();
        }

        private void AddAllWeaponsToItemList()
        {
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }
        }

        private void AssignItemsIds()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetItemID(i);
            }
        }

        public WeaponItem GetWeaponItemByID(int id)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == id);
        }
    }
}
