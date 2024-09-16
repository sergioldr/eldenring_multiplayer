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

        public void PlayTakeDamageSoundFX(float volume = 1, bool randomizePitch = true)
        {
            AudioClip takeDamageAudioClip = WorldSoundFXManager.Instance.GetTakeDamageSoundFX();
            audioSource.PlayOneShot(takeDamageAudioClip, volume);

            audioSource.pitch = 1f;

            if (randomizePitch)
            {
                audioSource.pitch = Random.Range(0.5f, 1);
            }
        }
    }
}

