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
            effect.ProcessEffect(characterManager);
        }
    }
}
