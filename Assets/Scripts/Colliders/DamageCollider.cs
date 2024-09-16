using System.Collections.Generic;
using UnityEngine;

namespace SL
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Damage Collider")]
        protected Collider damageCollider;

        [Header("Damage")]
        public float physicalDamage = 10f;
        public float magicDamage = 0f;
        public float fireDamage = 0f;
        public float lightningDamage = 0f;
        public float holyDamage = 0f;

        [Header("Poise")]
        public float poiseDamage = 0f;

        [Header("Contact Point")]
        protected Vector3 contactPoint;

        [Header("Characters Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        protected virtual void Awake()
        {
            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {

        }

        protected virtual void DamagetTarget(CharacterManager damageTarget)
        {

        }

        public virtual void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public virtual void DisableDamageCollider()
        {
            damageCollider.enabled = false;
            charactersDamaged.Clear();
        }

        public void SetWeaponDamage(float physicalDamage, float magicDamage, float fireDamage, float lightningDamage, float holyDamage)
        {
            this.physicalDamage = physicalDamage;
            this.magicDamage = magicDamage;
            this.fireDamage = fireDamage;
            this.lightningDamage = lightningDamage;
            this.holyDamage = holyDamage;
        }
    }
}
