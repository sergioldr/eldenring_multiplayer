using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SL
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager Instance { get; private set; }

        private PlayerManager playerManager;

        [Header("Save / Load Game")]
        [SerializeField] private bool saveGame = false;
        [SerializeField] private bool loadGame = false;

        [Header("World Scene Index")]
        [SerializeField] private int worldSceneIndex = 1;

        [Header("Save Data Writer")]
        private SaveFileDataWriter saveFileDataWriter;

        [Header("Current Character Data")]
        [SerializeField] private CharacterSlot currentCharacterSlot;
        [SerializeField] private CharacterSaveData currentCharacterData;
        private string fileName = "";

        [Header("Character Slots")]
        private CharacterSlot[] allSlots = (CharacterSlot[])Enum.GetValues(typeof(CharacterSlot));
        private List<CharacterSaveData> characterSavedDataList = new List<CharacterSaveData>();

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
            LoadAllCharactersProfiles();
        }

        private void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }

            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }

        private void DecideCharacterFileNameBaseOnCharacterSlot()
        {
            int characterSlotIndex = (int)currentCharacterSlot;
            fileName = DefaultGameValues.SAVE_FILE_NAME + characterSlotIndex + DefaultGameValues.SAVE_FILE_EXTENSION;
        }

        public void CreateNewGame()
        {

            // here we check if all slots are full
            if (characterSavedDataList.All(item => item != null))
            {
                TitleScreenMenuUI.Instance.DisplayNoAvailableSlotsPopUp();
                return;
            }


            // here we find the next available slot
            for (int i = 0; i < allSlots.Length; i++)
            {
                if (characterSavedDataList[i] == null)
                {
                    currentCharacterSlot = allSlots[i];
                    break;
                }
            }


            // this is the next available slot
            currentCharacterData = new CharacterSaveData();
            currentCharacterData.characterSlot = currentCharacterSlot;
            characterSavedDataList.Add(currentCharacterData);

            StartCoroutine(LoadWorldScene());
        }

        public void LoadGame()
        {
            DecideCharacterFileNameBaseOnCharacterSlot();

            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.SetSaveDataDirectoryPath(Application.persistentDataPath);
            saveFileDataWriter.SetSaveDataFileName(fileName);

            currentCharacterData = saveFileDataWriter.LoadSavedFile<CharacterSaveData>();

            StartCoroutine(LoadWorldScene());
        }

        public void SaveGame()
        {
            DecideCharacterFileNameBaseOnCharacterSlot();

            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.SetSaveDataDirectoryPath(Application.persistentDataPath);
            saveFileDataWriter.SetSaveDataFileName(fileName);

            playerManager.SaveCurrentCharacterData(ref currentCharacterData);

            saveFileDataWriter.CreateNewSaveDataFile(currentCharacterData);
        }

        public void DeleteSavedGame(CharacterSlot slot)
        {
            int slotIndex = (int)slot;
            characterSavedDataList[slotIndex] = null;
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.SetSaveDataDirectoryPath(Application.persistentDataPath);
            saveFileDataWriter.SetSaveDataFileName(DefaultGameValues.SAVE_FILE_NAME + slotIndex + DefaultGameValues.SAVE_FILE_EXTENSION);
            saveFileDataWriter.DeleteSavedDataFile();

        }

        private void LoadAllCharactersProfiles()
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.SetSaveDataDirectoryPath(Application.persistentDataPath);

            foreach (CharacterSlot slot in allSlots)
            {
                saveFileDataWriter.SetSaveDataFileName(DefaultGameValues.SAVE_FILE_NAME + (int)slot + DefaultGameValues.SAVE_FILE_EXTENSION);
                CharacterSaveData loadedCharacterData = saveFileDataWriter.LoadSavedFile<CharacterSaveData>();
                characterSavedDataList.Add(loadedCharacterData);
            }
        }

        public IEnumerator LoadWorldScene()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
            // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);
            playerManager.LoadCurrentCharacterData(ref currentCharacterData);

            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneIndex;
        }

        public CharacterSaveData GetCurrentCharacterData()
        {
            return currentCharacterData;
        }

        public void SetCharacterSlot(CharacterSlot slot)
        {
            currentCharacterSlot = slot;
        }

        public CharacterSaveData GetCharacterSavedDataFromSlot(CharacterSlot slot)
        {
            int slotIndex = (int)slot;
            return characterSavedDataList[slotIndex];
        }

        public void SetPlayerManager(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        public List<CharacterSaveData> GetAllCharacterSlots()
        {
            return characterSavedDataList;
        }
    }
}

