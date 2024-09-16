using UnityEngine;

namespace SL
{
    public class WeaponItem : Item
    {
        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        [SerializeField] public int strengthRequirement = 0;
        [SerializeField] public int dexterityRequirement = 0;
        [SerializeField] public int faithRequirement = 0;

        [Header("Weapon Stats")]
        [SerializeField] public int physicalDamage = 0;
        [SerializeField] public int magicalDamage = 0;
        [SerializeField] public int fireDamage = 0;
        [SerializeField] public int lightningDamage = 0;
        [SerializeField] public int holyDamage = 0;

        [Header("Weapon Stamina Costs")]
        [SerializeField] public int baseStaminaCost = 20;

        [Header("Weapon Poise Damage")]
        [SerializeField] public float poiseDamage = 10;

        [Header("Weapon Actions")]
        [SerializeField] public WeaponItemAction rightHandWeaponAction;
    }
}
