using UnityEngine;

namespace SL
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        private PlayerManager playerManager;

        public WeaponModelInstantiationSlot rightHandSlot { get; private set; }
        public WeaponModelInstantiationSlot leftHandSlot { get; private set; }

        [SerializeField] private WeaponManager rightWeaponManager;
        [SerializeField] private WeaponManager leftWeaponManager;

        private GameObject rightHandWeaponModel;
        private GameObject leftHandWeaponModel;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();

            InitializeWeaponSlots();
        }

        protected override void Start()
        {
            base.Start();

            LoadWeaponsOnBothHands();
        }

        private void InitializeWeaponSlots()
        {
            WeaponModelInstantiationSlot[] weaponsSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach (WeaponModelInstantiationSlot weaponSlot in weaponsSlots)
            {
                if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandSlot = weaponSlot;
                }
                else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
                {
                    leftHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponsOnBothHands()
        {
            LoadWeaponOnLeftHand();
            LoadWeaponOnRightHand();
        }

        public void SwitchRightWeapon()
        {
            if (!playerManager.IsOwner) return;

            playerManager.GetPlayerAnimationManager().PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

            playerInventoryManager.rightHandSlotIndex += 1;

            if (playerInventoryManager.rightHandSlotIndex < 0 || playerInventoryManager.rightHandSlotIndex > 2)
            {
                playerInventoryManager.rightHandSlotIndex = 0;

                float weaponCounnt = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = 0; i < playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    if (playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        weaponCounnt += 1;
                        if (firstWeapon == null)
                        {
                            firstWeapon = playerInventoryManager.weaponsInRightHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCounnt <= 1)
                {
                    playerInventoryManager.rightHandSlotIndex = -1;
                    selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                    playerManager.GetPlayerNetworkManager().currentRightHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    playerInventoryManager.rightHandSlotIndex = firstWeaponPosition;
                    playerManager.GetPlayerNetworkManager().currentRightHandWeaponID.Value = firstWeapon.itemID;
                }

                return;
            }

            foreach (WeaponItem weapon in playerInventoryManager.weaponsInRightHandSlots)
            {
                // If the weapon id is not the unarmed weapon
                if (playerInventoryManager.weaponsInRightHandSlots[playerInventoryManager.rightHandSlotIndex].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = playerInventoryManager.weaponsInRightHandSlots[playerInventoryManager.rightHandSlotIndex];
                    playerManager.GetPlayerNetworkManager().currentRightHandWeaponID.Value = playerInventoryManager.weaponsInRightHandSlots[playerInventoryManager.rightHandSlotIndex].itemID;
                    return;
                }
            }

            if (selectedWeapon == null && playerManager.GetPlayerInventoryManager().rightHandSlotIndex <= 2)
            {
                SwitchRightWeapon();
            }
        }

        public void SwitchLeftWeapon()
        {
            if (!playerManager.IsOwner) return;

            playerManager.GetPlayerAnimationManager().PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

            WeaponItem selectedWeapon = null;

            PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

            playerInventoryManager.leftHandSlotIndex += 1;

            if (playerInventoryManager.leftHandSlotIndex < 0 || playerInventoryManager.leftHandSlotIndex > 2)
            {
                playerInventoryManager.leftHandSlotIndex = 0;

                float weaponCounnt = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for (int i = 0; i < playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
                {
                    if (playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        weaponCounnt += 1;
                        if (firstWeapon == null)
                        {
                            firstWeapon = playerInventoryManager.weaponsInLeftHandSlots[i];
                            firstWeaponPosition = i;
                        }
                    }
                }

                if (weaponCounnt <= 1)
                {
                    playerInventoryManager.leftHandSlotIndex = -1;
                    selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                    playerManager.GetPlayerNetworkManager().currentLeftHandWeaponID.Value = selectedWeapon.itemID;
                }
                else
                {
                    playerInventoryManager.leftHandSlotIndex = firstWeaponPosition;
                    playerManager.GetPlayerNetworkManager().currentLeftHandWeaponID.Value = firstWeapon.itemID;
                }

                return;
            }

            foreach (WeaponItem weapon in playerInventoryManager.weaponsInLeftHandSlots)
            {
                // If the weapon id is not the unarmed weapon
                if (playerInventoryManager.weaponsInLeftHandSlots[playerInventoryManager.leftHandSlotIndex].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    selectedWeapon = playerInventoryManager.weaponsInLeftHandSlots[playerInventoryManager.leftHandSlotIndex];
                    playerManager.GetPlayerNetworkManager().currentLeftHandWeaponID.Value = playerInventoryManager.weaponsInLeftHandSlots[playerInventoryManager.leftHandSlotIndex].itemID;
                    return;
                }
            }

            if (selectedWeapon == null && playerManager.GetPlayerInventoryManager().rightHandSlotIndex <= 2)
            {
                SwitchLeftWeapon();
            }
        }

        public void LoadWeaponOnRightHand()
        {
            PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

            if (playerInventoryManager.currentRightHandWeapon != null)
            {
                rightHandSlot.UnloadWeapon();
                rightHandWeaponModel = Instantiate(playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                rightWeaponManager.SetWeaponDamage(playerManager, playerInventoryManager.currentRightHandWeapon);
            }
        }

        public void LoadWeaponOnLeftHand()
        {
            PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

            if (playerInventoryManager.currentLeftHandWeapon != null)
            {
                leftHandSlot.UnloadWeapon();
                leftHandWeaponModel = Instantiate(playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                leftWeaponManager.SetWeaponDamage(playerManager, playerInventoryManager.currentLeftHandWeapon);
            }
        }

        public void OpenDamageCollider()
        {
            if (playerManager.GetPlayerNetworkManager().isUsingRightHandWeapon.Value)
            {
                rightWeaponManager.meleeWDamageCollider.EnableDamageCollider();
            }
            else if (playerManager.GetPlayerNetworkManager().isUsingLeftHandWeapon.Value)
            {
                leftWeaponManager.meleeWDamageCollider.EnableDamageCollider();
            }

            // TODO: PLAY SFX HERE
        }

        public void CloseDamageCollider()
        {
            if (playerManager.GetPlayerNetworkManager().isUsingRightHandWeapon.Value)
            {
                rightWeaponManager.meleeWDamageCollider.DisableDamageCollider();
            }
            else if (playerManager.GetPlayerNetworkManager().isUsingLeftHandWeapon.Value)
            {
                leftWeaponManager.meleeWDamageCollider.DisableDamageCollider();
            }
        }
    }
}
