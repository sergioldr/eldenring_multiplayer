using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterManager : NetworkBehaviour
    {
        private CharacterController characterController;
        private CharacterNetworkManager characterNetworkManager;
        private Animator animator;

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

    }
}

