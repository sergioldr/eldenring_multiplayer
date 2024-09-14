using UnityEngine;

namespace SL
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        private PlayerManager playerManager;

        public WeaponModelInstantiationSlot rightHandSlot { get; private set; }
        public WeaponModelInstantiationSlot leftHandSlot { get; private set; }

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

        public void LoadWeaponOnRightHand()
        {
            PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

            if (playerInventoryManager.currentRightHandWeapon != null)
            {
                rightHandWeaponModel = Instantiate(playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
            }
        }

        public void LoadWeaponOnLeftHand()
        {
            PlayerInventoryManager playerInventoryManager = playerManager.GetPlayerInventoryManager();

            if (playerInventoryManager.currentLeftHandWeapon != null)
            {
                Debug.Log(playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandWeaponModel = Instantiate(playerInventoryManager.currentLeftHandWeapon.weaponModel);
                Debug.Log(leftHandWeaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
            }
        }
    }
}
