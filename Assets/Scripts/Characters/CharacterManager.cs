using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private Animator animator;
        private CharacterController characterController;
        private CharacterNetworkManager characterNetworkManager;
        private CharacterEffectsManager characterEffectsManager;

        [Header("Character Flags")]
        [SerializeField] private bool isPerformingAction = false;
        [SerializeField] private bool applyRootMotion = false;
        [SerializeField] private bool canRotate = true;
        [SerializeField] private bool canMove = true;
        [SerializeField] private bool isJumping = false;
        [SerializeField] private bool isGrounded = true;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
        }

        protected virtual void Update()
        {
            animator.SetBool("IsGrounded", isGrounded);

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

        // Getters and Setters
        public Animator GetCharacterAnimator()
        {
            return animator;
        }

        public CharacterNetworkManager GetCharacterNetworkManager()
        {
            return characterNetworkManager;
        }

        public CharacterController GetCharacterController()
        {
            return characterController;
        }

        public CharacterEffectsManager GetCharacterEffectsManager()
        {
            return characterEffectsManager;
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

        public bool GetIsJumping()
        {
            return isJumping;
        }

        public void SetIsJumping(bool value)
        {
            isJumping = value;
        }

        public bool GetIsGrounded()
        {
            return isGrounded;
        }

        public void SetIsGrounded(bool value)
        {
            isGrounded = value;
        }

        public bool GetIsDead()
        {
            return isDead.Value;
        }

        public void SetIsDead(bool value)
        {
            isDead.Value = value;
        }
    }
}

