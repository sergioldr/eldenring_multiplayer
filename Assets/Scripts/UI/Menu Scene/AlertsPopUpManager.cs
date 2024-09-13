using UnityEngine;
using UnityEngine.UI;

namespace SL
{
    public class AlertsPopUpManager : MonoBehaviour
    {
        public static AlertsPopUpManager Instance { get; private set; }

        private PlayerControls playerControls;

        [Header("Alerts Components")]
        [SerializeField] private GameObject noAvailableSlotsPopUp;
        [SerializeField] private GameObject deleteCharacterSlotPopUp;

        [Header("Alerts Buttons")]
        [SerializeField] private Button noAvailableSlotsPopUpButton;
        [SerializeField] private Button deleteCharacterSlotCancellButton;

        private bool isAlertActive = false;

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

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                playerControls.UI.DeleteSavedGame.performed += ctx => ShowDeleteCharacterSlotPopUp();
            }

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        public void ShowNoAvailableSlotsPopUp()
        {
            isAlertActive = true;
            noAvailableSlotsPopUp.SetActive(true);
            noAvailableSlotsPopUpButton.Select();
        }

        public void HideNoAvailableSlotsPopUp()
        {
            noAvailableSlotsPopUp.SetActive(false);
            isAlertActive = false;
        }

        public void ShowDeleteCharacterSlotPopUp()
        {
            isAlertActive = true;
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterSlotCancellButton.Select();
        }

        public void HideDeleteCharacterSlotPopUp()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            isAlertActive = false;
        }

        public bool IsAlertActive()
        {
            return isAlertActive;
        }

    }
}
