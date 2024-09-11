using UnityEngine;

namespace SL
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        private AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayRollSoundFX()
        {
            AudioClip rollAudioClip = WorldSoundFXManager.Instance.GetRollSoundFX();
            audioSource.PlayOneShot(rollAudioClip);
        }
    }
}

