using UnityEngine;

namespace SL
{
    [CreateAssetMenu(menuName = "Character/Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        [SerializeField] private CharacterManager characterCausingDamage;

        [Header("Damage Options")]
        [SerializeField] private float physicalDamage = 10f;
        [SerializeField] private float magicDamage = 0f;
        [SerializeField] private float fireDamage = 0f;
        [SerializeField] private float lightningDamage = 0f;
        [SerializeField] private float holyDamage = 0f;

        [Header("Final Damage")]
        private int finalDamage = 0;

        [Header("Poise")]
        [SerializeField] private float poiseDamage = 0f;
        [SerializeField] private bool poiseIsBroken = false;

        [Header("Animation")]
        [SerializeField] private bool playDamageAnimation = true;
        [SerializeField] private bool manuallySelectDamageAnimation = false;
        [SerializeField] private string damageAnimation = "TakeDamage";

        [Header("Sound FX")]
        [SerializeField] private bool playDamageSFX = true;
        [SerializeField] private AudioClip damageSFX;

        [Header("Direction Damage Taken From")]
        [SerializeField] private float angleHitFrom = 0f;
        [SerializeField] private Vector3 contactPoint;

        public override void ProcessEffect(CharacterManager characterManager)
        {
            base.ProcessEffect(characterManager);

            if (characterManager.GetIsDead()) return;

            CalculateDamage(characterManager);
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner) return;
            if (characterCausingDamage != null) return;

            Debug.Log("Calculating Damageeeeee");

            finalDamage = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamage <= 0)
            {
                finalDamage = 1;
            };

            Debug.Log("Final Damage: " + finalDamage);

            character.GetCharacterNetworkManager().networkCurrentHealth.Value -= finalDamage;
        }

        public void SetDamagesEffects(float physical, float magic, float fire, float lightning, float holy)
        {
            physicalDamage = physical;
            magicDamage = magic;
            fireDamage = fire;
            lightningDamage = lightning;
            holyDamage = holy;
        }
    }
}
