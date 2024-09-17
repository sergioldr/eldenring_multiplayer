using UnityEngine;

namespace SL
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        [Header("VFX")]
        [SerializeField] private GameObject bloodSplatterVFX;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        public void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            effect.ProcessEffect(characterManager);
        }

        public void PlayBloodVFX(Vector3 contactPoint)
        {
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.Instance.bloodSplatterVFX, contactPoint, Quaternion.identity) as GameObject;
            }
        }
    }
}
