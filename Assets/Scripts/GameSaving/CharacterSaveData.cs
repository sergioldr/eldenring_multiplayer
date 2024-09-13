using UnityEngine;

namespace SL
{
    [System.Serializable]
    public class CharacterSaveData
    {
        [Header("Character Slot")]
        public CharacterSlot characterSlot;

        [Header("Scene Index")]
        public int sceneIndex = 1;

        [Header("Character Name")]
        public string characterName = "New Character";

        [Header("Time Played")]
        public float secondsPlayed;

        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;
    }
}

