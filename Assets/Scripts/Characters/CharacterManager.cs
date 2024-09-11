using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterManager : NetworkBehaviour
    {
        private Animator animator;
        private CharacterController characterController;
        private CharacterNetworkManager characterNetworkManager;

        [Header("Character Flags")]
        [SerializeField] private bool isPerformingAction = false;
        [SerializeField] private bool applyRootMotion = false;
        [SerializeField] private bool canRotate = true;
        [SerializeField] private bool canMove = true;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            animator = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            else
            {

                //Player position for network character
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionPositionSmoothTime
                );
                //Player rotation for network character
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    characterNetworkManager.networkRotation.Value,
                    characterNetworkManager.networkPositionRotationSmoothTime
                );
            }
        }

        protected virtual void LateUpdate()
        {

        }

        public CharacterNetworkManager GetCharacterNetworkManager()
        {
            return characterNetworkManager;
        }

        public CharacterController GetCharacterController()
        {
            return characterController;
        }

        public Animator GetCharacterAnimator()
        {
            return animator;
        }

        public bool GetApplyRootMotion()
        {
            return applyRootMotion;
        }

        public void SetApplyRootMotion(bool value)
        {
            applyRootMotion = value;
        }

        public bool GetIsPerformingAction()
        {
            return isPerformingAction;
        }

        public void SetIsPerformingAction(bool value)
        {
            isPerformingAction = value;
        }

        public bool GetCanRotate()
        {
            return canRotate;
        }

        public void SetCanRotate(bool value)
        {
            canRotate = value;
        }

        public bool GetCanMove()
        {
            return canMove;
        }

        public void SetCanMove(bool value)
        {
            canMove = value;
        }

    }
}

