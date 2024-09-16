using UnityEngine;

namespace SL
{
    public class WeaponItem : Item
    {
        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        public int strengthRequirement = 0;
        public int dexterityRequirement = 0;
        public int faithRequirement = 0;

        [Header("Weapon Stats")]
        public int physicalDamage = 0;
        public int magicalDamage = 0;
        public int fireDamage = 0;
        public int lightningDamage = 0;
        public int holyDamage = 0;

        [Header("Attack Modifiers")]
        public float lightAttackModifier = 1.1f;

        [Header("Weapon Stamina Costs")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostMultiplier = 0.9f;

        [Header("Weapon Poise Damage")]
        public float poiseDamage = 10;

        [Header("Weapon Actions")]
        public WeaponItemAction rightHandWeaponAction;
    }
}
