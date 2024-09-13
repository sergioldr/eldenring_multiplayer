using UnityEngine;

namespace SL
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        public void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            Debug.Log("CharacterEffectsManager: Processing Instant Effect");
            effect.ProcessEffect(characterManager);
        }
    }
}
