using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SL
{
    public class PlayerManager : CharacterManager
    {
        [Header("DEBUG MENU")]
        [SerializeField] private bool respawnCharacter = false;
        [SerializeField] private bool switchRightWeapon = false;

        private PlayerLocomotionManager playerLocomotionManager;
        private PlayerAnimationManager playerAnimationManager;
        private PlayerNetworkManager playerNetworkManager;
        private PlayerStatsManager playerStatsManager;
        private PlayerInventoryManager playerInventoryManager;
        private PlayerEquipmentManager playerEquipmentManager;

        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimationManager = GetComponent<PlayerAnimationManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (!IsOwner) return;

            playerLocomotionManager.HandleAllMovement();
            playerStatsManager.RegenerateStamina();

            DebugMenu();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!IsOwner) return;

            PlayerCamera.Instance.HandleAllCameraActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PlayerCamera.Instance.SetPlayerManager(this);
                PlayerInputManager.Instance.SetPlayerManager(this);
                WorldSaveGameManager.Instance.SetPlayerManager(this);

                PlayerUIHUDManager playerUIHUDManager = PlayerUIManager.Instance.GetPlayerUIHUDManager();

                // THIS UPDATES THE TOTAL AMOUNT OF HEALTH OR STAMINA WHEN THE STAT LINKED TO EITHER CHANGES (VITALITY, ENDURANCE)
                playerNetworkManager.networkVitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.networkEndurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

                // THIS UPDATES UI STATS BARS WHEN A STAT CHANGES (HEALTH, STAMINA)
                playerNetworkManager.networkCurrentHealth.OnValueChanged += playerUIHUDManager.SetNewHealthValue;
                playerNetworkManager.networkCurrentStamina.OnValueChanged += playerUIHUDManager.SetNewStaminaValue;
                playerNetworkManager.networkCurrentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenerationTimer;
            }

            playerNetworkManager.networkCurrentHealth.OnValueChanged += playerNetworkManager.CheckHealth;

            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChanged;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChanged;
        }

        public override IEnumerator ProcessDeathEvent(bool selectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.Instance.GetPlayerUIPopUpManager().SendYouDiedPopUp();
            }

            return base.ProcessDeathEvent(selectDeathAnimation);
        }

        public override void RespawnCharacter()
        {
            base.RespawnCharacter();

            if (IsOwner)
            {
                playerNetworkManager.networkCurrentHealth.Value = playerNetworkManager.networkMaxHealth.Value;
                playerNetworkManager.networkCurrentStamina.Value = playerNetworkManager.networkMaxStamina.Value;

                // Reset player animation
                playerAnimationManager.PlayTargetActionAnimation("Empty", true);
            }
        }

        public void SaveCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.networkCurrentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.networkCurrentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.networkVitality.Value;
            currentCharacterData.endurance = playerNetworkManager.networkEndurance.Value;
        }

        public void LoadCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(
                currentCharacterData.xPosition,
                currentCharacterData.yPosition,
                currentCharacterData.zPosition
            );
            transform.position = myPosition;

            // Set the player's vitality and endurance based on the character's data
            playerNetworkManager.networkVitality.Value = currentCharacterData.vitality;
            playerNetworkManager.networkEndurance.Value = currentCharacterData.endurance;

            // Set the player's health and stamina based on the character's vitality and endurance
            PlayerUIHUDManager playerUIHUDManager = PlayerUIManager.Instance.GetPlayerUIHUDManager();
            playerNetworkManager.networkMaxHealth.Value = playerStatsManager.CalculateBaseHealthBasedOnVitalityLevel(playerNetworkManager.networkVitality.Value);
            playerNetworkManager.networkMaxStamina.Value = playerStatsManager.CalculateBaseStaminaBasedOnEnduranceLevel(playerNetworkManager.networkEndurance.Value);
            playerNetworkManager.networkCurrentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.networkCurrentStamina.Value = currentCharacterData.currentStamina;
            playerUIHUDManager.SetMaxStaminaValue(playerNetworkManager.networkMaxStamina.Value);
        }

        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                RespawnCharacter();
            }

            if (switchRightWeapon)
            {
                switchRightWeapon = false;
                playerEquipmentManager.SwitchRightWeapon();
            }
        }

        // GETTERS AND SETTERS

        public PlayerAnimationManager GetPlayerAnimationManager()
        {
            return playerAnimationManager;
        }

        public PlayerLocomotionManager GetPlayerLocomotionManager()
        {
            return playerLocomotionManager;
        }

        public PlayerNetworkManager GetPlayerNetworkManager()
        {
            return playerNetworkManager;
        }

        public PlayerStatsManager GetPlayerStatsManager()
        {
            return playerStatsManager;
        }

        public PlayerInventoryManager GetPlayerInventoryManager()
        {
            return playerInventoryManager;
        }

        public PlayerEquipmentManager GetPlayerEquipmentManager()
        {
            return playerEquipmentManager;
        }
    }
}
