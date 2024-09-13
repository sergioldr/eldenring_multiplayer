using System.Collections.Generic;
using UnityEngine;

namespace SL
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private float physicalDamage = 10f;
        [SerializeField] private float magicDamage = 0f;
        [SerializeField] private float fireDamage = 0f;
        [SerializeField] private float lightningDamage = 0f;
        [SerializeField] private float holyDamage = 0f;

        [Header("Contact Point")]
        private Vector3 contactPoint;

        [Header("Characters Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        private void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponent<CharacterManager>();

            if (damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                DamagetTarget(damageTarget);
            }
        }

        protected virtual void DamagetTarget(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget)) return;

            charactersDamaged.Add(damageTarget);

            TakeDamageEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.Instance.GetTakeDamageEffect());
            takeDamageEffect.SetDamagesEffects(physicalDamage, magicDamage, fireDamage, lightningDamage, holyDamage);

            damageTarget.GetCharacterEffectsManager().ProcessInstantEffect(takeDamageEffect);
        }
    }
}
