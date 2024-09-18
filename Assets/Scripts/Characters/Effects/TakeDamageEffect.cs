using UnityEngine;

namespace SL
{
    [CreateAssetMenu(menuName = "Character/Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        [SerializeField] private CharacterManager characterCausingDamage;

        [Header("Damage Options")]
        public float physicalDamage = 10f;
        public float magicDamage = 0f;
        public float fireDamage = 0f;
        public float lightningDamage = 0f;
        public float holyDamage = 0f;

        [Header("Final Damage")]
        private int finalDamage = 0;

        [Header("Poise")]
        public float poiseDamage = 0f;
        public bool poiseIsBroken = false;

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")]
        public bool playDamageSFX = true;
        public AudioClip damageSFX;

        [Header("Direction Damage Taken From")]
        public float angleHitFrom = 0f;
        public Vector3 contactPoint;

        public override void ProcessEffect(CharacterManager characterManager)
        {
            base.ProcessEffect(characterManager);

            if (characterManager.GetIsDead()) return;

            CalculateDamage(characterManager);
            PlayDirectionalDamageAnimation(characterManager);

            PlayDamageSFX(characterManager);
            PlayDamageVFX(characterManager);
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner) return;

            if (characterCausingDamage != null)
            {
                // CHECK FOR DAMAGE MODIFIERS
            }

            finalDamage = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamage <= 0)
            {
                finalDamage = 1;
            };

            Debug.Log("Character: " + character.name + " took " + finalDamage + " damage");

            character.GetCharacterNetworkManager().networkCurrentHealth.Value -= finalDamage;
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            // IF WE HAVE FIRE DAMAGE, PLAY FIRE VFX
            // IF WE HAVE LIGHTNING DAMAGE, PLAY LIGHTNING VFX

            character.GetCharacterEffectsManager().PlayBloodVFX(contactPoint);
        }

        private void PlayDamageSFX(CharacterManager character)
        {
            // PLAY DAMAGE SFX
            character.GetCharacterSoundFXManager().PlayTakeDamageSoundFX();
        }

        private void PlayDirectionalDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner) return;

            if (character.GetIsDead()) return;

            CharacterAnimationManager animator = character.GetCharacterAnimationManager();

            // TODO: CALCULATE IF POISE IS BROKEN
            poiseIsBroken = true;

            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                // PLAY FRONT DAMAGE ANIMATION
                damageAnimation = animator.GetRandomAnimation(animator.forward_medium_damage);
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                // PLAY FRONT DAMAGE ANIMATION
                damageAnimation = animator.GetRandomAnimation(animator.forward_medium_damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                // PLAY BACK DAMAGE ANIMATION
                damageAnimation = animator.GetRandomAnimation(animator.backward_medium_damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= 45)
            {
                // PLAY LEFT DAMAGE ANIMATION
                damageAnimation = animator.GetRandomAnimation(animator.left_medium_damage);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                // PLAY RIGHT DAMAGE ANIMATION
                damageAnimation = animator.GetRandomAnimation(animator.right_medium_damage);
            }

            if (poiseIsBroken)
            {
                animator.lastDamageAnimationPlayed = damageAnimation;
                animator.PlayTargetActionAnimation(damageAnimation, true);
            }
        }

        public void SetDamagesEffects(float physical, float magic, float fire, float lightning, float holy, float poise)
        {
            physicalDamage = physical;
            magicDamage = magic;
            fireDamage = fire;
            lightningDamage = lightning;
            holyDamage = holy;
            poiseDamage = poise;
        }

        public void SetDamageModifiers(float modifier)
        {
            physicalDamage *= modifier;
            magicDamage *= modifier;
            fireDamage *= modifier;
            lightningDamage *= modifier;
            holyDamage *= modifier;
        }

        public void SetDamageDirection(
            float angle,
            float contactPointX,
            float contactPointY,
            float contactPointZ
        )
        {
            angleHitFrom = angle;
            contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
        }

        public void SetCharacterCausingDamage(CharacterManager character)
        {
            characterCausingDamage = character;
        }
    }
}
