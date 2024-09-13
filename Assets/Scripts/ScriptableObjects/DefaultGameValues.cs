using UnityEngine;

namespace SL
{
    [CreateAssetMenu()]
    public class DefaultGameValues : ScriptableObject
    {
        public const string GAME_VERSION = "0.0.1";
        public const string GAME_NAME = "SL";
        public const string SAVE_FILE_EXTENSION = ".json";
        public const string SAVE_FILE_NAME = "CharacterSlot_";
    }
}

