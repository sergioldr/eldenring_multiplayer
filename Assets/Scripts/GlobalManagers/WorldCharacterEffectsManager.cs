using System.Collections.Generic;
using UnityEngine;

namespace SL
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        public static WorldCharacterEffectsManager Instance { get; private set; }

        [Header("Damage")]
        [SerializeField] private TakeDamageEffect takeDamageEffect;

        [SerializeField] private List<InstantCharacterEffect> instantCharacterEffects = new List<InstantCharacterEffect>();

        [Header("VFX")]
        public GameObject bloodSplatterVFX;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            GenerateEffectIDs();
        }

        private void GenerateEffectIDs()
        {
            for (int i = 0; i < instantCharacterEffects.Count; i++)
            {
                instantCharacterEffects[i].instantEffectID = i;
            }
        }

        public TakeDamageEffect GetTakeDamageEffect()
        {
            return takeDamageEffect;
        }
    }
}
