using UnityEngine;

namespace SL
{
    public class WeaponItem : Item
    {
        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        [SerializeField] private int strengthRequirement = 0;
        [SerializeField] private int dexterityRequirement = 0;
        [SerializeField] private int faithRequirement = 0;

        [Header("Weapon Stats")]
        [SerializeField] private int physicalDamage = 0;
        [SerializeField] private int magicalDamage = 0;
        [SerializeField] private int fireDamage = 0;
        [SerializeField] private int lightningDamage = 0;
        [SerializeField] private int holyDamage = 0;

        [Header("Stamina Costs")]
        [SerializeField] private int baseStaminaCost = 20;

        [Header("Weapon Poise Damage")]
        [SerializeField] private float poiseDamage = 10;
    }
}
