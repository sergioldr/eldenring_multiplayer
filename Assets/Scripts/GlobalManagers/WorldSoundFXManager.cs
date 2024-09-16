using UnityEngine;

namespace SL
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager Instance { get; private set; }

        [Header("Action Sound")]
        [SerializeField] private AudioClip rollSoundSFX;

        [Header("Damage Sounds")]
        [SerializeField] private AudioClip[] takeDamageSoundListSFX;

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
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public AudioClip GetRollSoundFX()
        {
            return rollSoundSFX;
        }

        public AudioClip GetTakeDamageSoundFX()
        {
            return takeDamageSoundListSFX[Random.Range(0, takeDamageSoundListSFX.Length)];
        }
    }
}
