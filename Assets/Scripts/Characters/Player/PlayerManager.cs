using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace SL
{
    public class PlayerManager : CharacterManager
    {
        private PlayerLocomotionManager playerLocomotionManager;
        private PlayerAnimationManager playerAnimationManager;
        private PlayerNetworkManager playerNetworkManager;
        private PlayerStatsManager playerStatsManager;
        private PlayerInventoryManager playerInventoryManager;
        private PlayerEquipmentManager playerEquipmentManager;
        private PlayerCombatManager playerCombatManager;

        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimationManager = GetComponent<PlayerAnimationManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (!IsOwner) return;

            playerLocomotionManager.HandleAllMovement();
            playerStatsManager.RegenerateStamina();
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

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

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

            // STATS
            playerNetworkManager.networkCurrentHealth.OnValueChanged += playerNetworkManager.CheckHealth;

            // LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChanged;


            // EQUIPMENT
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChanged;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChanged;
            playerNetworkManager.currentWeaponBeingUsedID.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChanged;

            // FLAGS
            playerNetworkManager.isChargingHeavyAttack.OnValueChanged += playerNetworkManager.OnIsChargingHeavyAttackChanged;

            // UPON CONNECTING TO THE SERVER, LOAD THE CHARACTER DATA FOR CLIENTS
            // WE ONLY WANT TO DO THIS FOR CLIENTS, NOT THE SERVER. THE SERVER WILL LOAD THE CHARACTER DATA WHEN THE GAME STARTS
            if (IsOwner && !IsServer)
            {
                CharacterSaveData currentCharacterData = WorldSaveGameManager.Instance.GetAllCharacterSlots()[playerNetworkManager.networkCharacterSlot.Value];
                LoadCurrentCharacterData(ref currentCharacterData);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

            if (IsOwner)
            {

                PlayerUIHUDManager playerUIHUDManager = PlayerUIManager.Instance.GetPlayerUIHUDManager();

                // THIS UPDATES THE TOTAL AMOUNT OF HEALTH OR STAMINA WHEN THE STAT LINKED TO EITHER CHANGES (VITALITY, ENDURANCE)
                playerNetworkManager.networkVitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.networkEndurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

                // THIS UPDATES UI STATS BARS WHEN A STAT CHANGES (HEALTH, STAMINA)
                playerNetworkManager.networkCurrentHealth.OnValueChanged -= playerUIHUDManager.SetNewHealthValue;
                playerNetworkManager.networkCurrentStamina.OnValueChanged -= playerUIHUDManager.SetNewStaminaValue;
                playerNetworkManager.networkCurrentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenerationTimer;
            }

            // STATS
            playerNetworkManager.networkCurrentHealth.OnValueChanged -= playerNetworkManager.CheckHealth;

            // LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChanged;


            // EQUIPMENT
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChanged;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChanged;
            playerNetworkManager.currentWeaponBeingUsedID.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChanged;

            // FLAGS
            playerNetworkManager.isChargingHeavyAttack.OnValueChanged -= playerNetworkManager.OnIsChargingHeavyAttackChanged;
        }

        private void OnClientConnectedCallback(ulong clientID)
        {
            if (!IsServer && IsOwner)
            {
                foreach (PlayerManager player in WorldGameSessionManager.Instance.activePlayers)
                {
                    if (player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
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
                SetIsDead(false);
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

        private void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            Debug.Log("Loading other player character data");
            playerNetworkManager.OnCurrentRightHandWeaponIDChanged(0, playerNetworkManager.currentRightHandWeaponID.Value);
            playerNetworkManager.OnCurrentLeftHandWeaponIDChanged(0, playerNetworkManager.currentLeftHandWeaponID.Value);

            if (playerNetworkManager.isLockedOn.Value)
            {
                playerNetworkManager.OnLockOnTargetIDChanged(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
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

        public PlayerCombatManager GetPlayerCombatManager()
        {
            return playerCombatManager;
        }
    }
}
