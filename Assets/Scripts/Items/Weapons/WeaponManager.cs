using UnityEngine;

namespace SL
{
    public class WeaponManager : MonoBehaviour
    {
        public MeleeWeaponDamageCollider meleeWDamageCollider { get; private set; }

        private void Awake()
        {
            meleeWDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
        {
            meleeWDamageCollider.characterCausingDamage = characterWieldingWeapon;
            meleeWDamageCollider.SetWeaponDamage(
                weapon.physicalDamage,
                weapon.magicalDamage,
                weapon.fireDamage,
                weapon.lightningDamage,
                weapon.holyDamage
            );

            meleeWDamageCollider.lightAttackModifier = weapon.lightAttackModifier;
        }
    }
}
