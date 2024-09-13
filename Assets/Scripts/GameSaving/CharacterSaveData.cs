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

        [Header("Character Stats")]
        public int vitality;
        public int endurance;

        [Header("Character Resources")]
        public float currentHealth;
        public int maxHealth;
        public float currentStamina;
        public int maxStamina;
    }
}

