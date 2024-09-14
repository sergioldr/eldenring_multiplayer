using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        private CharacterManager characterManager;

        [Header("Position")]
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionPositionSmoothTime = 0.1f;
        public float networkPositionRotationSmoothTime = 0.1f;

        [Header("Animator")]
        public NetworkVariable<float> networkHorizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> networkVerticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> networkAmountMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Resources")]
        public NetworkVariable<int> networkCurrentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> networkMaxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> networkCurrentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> networkMaxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> networkVitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> networkEndurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        public void CheckHealth(int oldValue, int newValue)
        {
            Debug.Log("Health Changed: " + oldValue + " -> " + newValue);
            if (networkCurrentHealth.Value <= 0)
            {
                StartCoroutine(characterManager.ProcessDeathEvent());
            }

            // Prevetns the health from going over the max health
            if (characterManager.IsOwner && networkCurrentHealth.Value > networkMaxHealth.Value)
            {
                networkCurrentHealth.Value = networkMaxHealth.Value;
            }
        }

        // Server RPCs are a way to send a message from the client to the server
        [ServerRpc]
        public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // If this character is the HOST/Server then play the animation for all clients in all clients rpcs
            if (IsServer)
            {
                PlayActionANimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayActionANimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            characterManager.SetApplyRootMotion(applyRootMotion);
            characterManager.GetCharacterAnimator().CrossFade(animationID, 0.2f);
        }
    }
}