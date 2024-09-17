using UnityEngine;

namespace SL
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage;

        [Header("Weapon Attack Modifier")]
        public float lightAttackModifier;

        protected override void Awake()
        {
            base.Awake();

            damageCollider.enabled = false;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            // IF YOU WANT TO SEARCH ON BOTH THE DAMAGEABLE CHARACTER COLLIDERS AND THE CHARACTER CONTROLLER COLLIDERS

            // if (damageTarget == null)
            // {
            //     damageTarget = other.GetComponent<CharacterManager>();
            // }

            if (damageTarget != null)
            {
                if (damageTarget == characterCausingDamage) return;

                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                DamagetTarget(damageTarget);
            }
        }

        protected override void DamagetTarget(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget)) return;

            charactersDamaged.Add(damageTarget);

            TakeDamageEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.Instance.GetTakeDamageEffect());
            takeDamageEffect.SetDamagesEffects(physicalDamage, magicDamage, fireDamage, lightningDamage, holyDamage, poiseDamage);

            float angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
            takeDamageEffect.angleHitFrom = angleHitFrom;


            switch (characterCausingDamage.GetCharacterCombatManager().currentAttackType)
            {
                case AttackType.LightAttack:
                    ApplyAttackDamageModifiers(lightAttackModifier, takeDamageEffect);
                    break;
                default:
                    break;
            }

            //damageTarget.GetCharacterEffectsManager().ProcessInstantEffect(takeDamageEffect);

            if (characterCausingDamage.IsOwner)
            {
                damageTarget.GetCharacterNetworkManager().NotifyTheServerOfDamageActionAnimationServerRpc(
                    characterCausingDamage.NetworkObjectId,
                    damageTarget.NetworkObjectId,
                    takeDamageEffect.physicalDamage,
                    takeDamageEffect.magicDamage,
                    takeDamageEffect.fireDamage,
                    takeDamageEffect.lightningDamage,
                    takeDamageEffect.holyDamage,
                    takeDamageEffect.poiseDamage,
                    takeDamageEffect.angleHitFrom,
                    takeDamageEffect.contactPoint.x,
                    takeDamageEffect.contactPoint.y,
                    takeDamageEffect.contactPoint.z
                );
            }
        }

        private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
        {
            damage.SetDamageModifiers(modifier);

            // IF ATTACK IS A FULLY CHARGED HEAVYU, MULTIPLY BY FULL CHARGE MODIFIER AFTER NORMAL MODIFIER HAVE BEEN CALCULATED
        }
    }
}
