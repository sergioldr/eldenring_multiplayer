using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SL
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {

        private SaveFileDataWriter saveFileWriter;

        [SerializeField] private Button loadGameButton;

        [Header("Game Slot")]
        [SerializeField] private CharacterSlot characterSlot;

        [Header("Character Info")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI timePlayedText;

        private void Update()
        {
            if (AlertsPopUpManager.Instance.IsAlertActive())
            {
                loadGameButton.interactable = false;
            }
            else
            {
                loadGameButton.interactable = true;
            }
        }

        private void OnEnable()
        {
            LoadSaveSlots();
        }

        private void LoadSaveSlots()
        {
            saveFileWriter = new SaveFileDataWriter();
            saveFileWriter.SetSaveDataDirectoryPath(Application.persistentDataPath);

            string fileName = DefaultGameValues.SAVE_FILE_NAME + (int)characterSlot + DefaultGameValues.SAVE_FILE_EXTENSION;
            saveFileWriter.SetSaveDataFileName(fileName);


            if (saveFileWriter.CheckIfSaveDataFileExists())
            {
                CharacterSaveData characterData = WorldSaveGameManager.Instance.GetCharacterSavedDataFromSlot(characterSlot);
                if (characterData != null)
                {
                    timePlayedText.text = characterSlot.ToString();
                    characterNameText.text = characterData.characterName;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.Instance.SetCharacterSlot(characterSlot);
            WorldSaveGameManager.Instance.LoadGame();
        }

        public void SelectCurrentCharacterSlot()
        {
            TitleScreenMenuUI.Instance.SetSlectedCharacterSlot(characterSlot);
        }

        public void SetCharacterSlot(CharacterSlot slot)
        {
            characterSlot = slot;
        }
    }
}
