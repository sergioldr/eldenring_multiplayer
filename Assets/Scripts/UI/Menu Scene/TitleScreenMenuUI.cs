using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;
using SL;
using UnityEngine.UI;

public class TitleScreenMenuUI : MonoBehaviour
{
    public static TitleScreenMenuUI Instance { get; private set; }

    [Header("Start Menu UI")]
    [SerializeField] private GameObject titleScreenMainMenu;
    [SerializeField] private GameObject loadCharacterMenu;
    [SerializeField] private Button mainMenuStartNewGameButton;
    [SerializeField] private Button mainMenuLoadGameButton;

    [Header("Load Games UI")]
    [SerializeField] private Transform savedGamesContainer;
    [SerializeField] private Transform gameSlotUI;
    [SerializeField] private CharacterSlot? selectedCharacterSlot;
    [SerializeField] private Button loadMenuReturnButton;
    [SerializeField] private Button deleteCharacterSlotConfirmButton;

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
        InstantiateAllSavedGames();
    }

    private void InstantiateAllSavedGames()
    {
        List<CharacterSaveData> savedDataList = WorldSaveGameManager.Instance.GetAllCharacterSlots();

        foreach (CharacterSaveData data in savedDataList)
        {
            if (data == null) continue;
            Transform slotUI = Instantiate(gameSlotUI, savedGamesContainer);
            slotUI.GetComponent<UI_Character_Save_Slot>().SetCharacterSlot(data.characterSlot);
        }
    }

    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.Instance.CreateNewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        loadCharacterMenu.SetActive(true);
        loadMenuReturnButton.Select();
    }

    public void CloseLoadGameMenu()
    {
        loadCharacterMenu.SetActive(false);
        titleScreenMainMenu.SetActive(true);
        mainMenuLoadGameButton.Select();
    }

    public void DisplayNoAvailableSlotsPopUp()
    {
        mainMenuLoadGameButton.interactable = false;
        mainMenuStartNewGameButton.interactable = false;
        AlertsPopUpManager.Instance.ShowNoAvailableSlotsPopUp();
    }

    public void CloseNoAvailableSlotsPopUp()
    {
        AlertsPopUpManager.Instance.HideNoAvailableSlotsPopUp();
        mainMenuLoadGameButton.interactable = true;
        mainMenuStartNewGameButton.interactable = true;
        mainMenuStartNewGameButton.Select();
    }

    public void DeselectCurrentCharacterSlot()
    {
        selectedCharacterSlot = null;
    }

    public void AttemptToDeleteCharacterSlot()
    {
        if (selectedCharacterSlot == null) return;

        loadMenuReturnButton.interactable = false;
        AlertsPopUpManager.Instance.ShowDeleteCharacterSlotPopUp();
    }

    public void ConfirmDeleteCharacterSlot()
    {
        if (selectedCharacterSlot == null) return;
        WorldSaveGameManager.Instance.DeleteSavedGame(selectedCharacterSlot.Value);
        loadCharacterMenu.gameObject.SetActive(false);
        loadCharacterMenu.gameObject.SetActive(true);
        CloseDeleteCharacterSlotPopUp();
    }

    public void CloseDeleteCharacterSlotPopUp()
    {
        AlertsPopUpManager.Instance.HideDeleteCharacterSlotPopUp();
        loadMenuReturnButton.interactable = true;
        loadMenuReturnButton.Select();
    }

    public void SetSlectedCharacterSlot(CharacterSlot slot)
    {
        selectedCharacterSlot = slot;
    }

    public CharacterSlot? GetSelectedCharacterSlot()
    {
        return selectedCharacterSlot;
    }
}
