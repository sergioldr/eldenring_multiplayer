using UnityEngine;

namespace SL
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager Instance { get; private set; }

        [Header("Action Sound FX")]
        [SerializeField] private AudioClip rollSoundFX;

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
            return rollSoundFX;
        }
    }
}
