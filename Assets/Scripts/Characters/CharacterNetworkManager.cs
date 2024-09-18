using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        private CharacterManager characterManager;

        [Header("Character Slot")]
        public NetworkVariable<int> networkCharacterSlot = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


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

        [Header("Target")]
        public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isChargingHeavyAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

        public void OnLockOnTargetIDChanged(ulong oldID, ulong newID)
        {
            if (!IsOwner)
            {
                characterManager.GetCharacterCombatManager().currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
            }
        }

        public void OnIsLockedOnChanged(bool oldIsLockedOn, bool newIsLockedOn)
        {
            if (!newIsLockedOn)
            {
                characterManager.GetCharacterCombatManager().currentTarget = null;
            }
        }

        public void OnIsChargingHeavyAttackChanged(bool oldIsChargingHeavyAttack, bool newIsChargingHeavyAttack)
        {
            characterManager.GetCharacterAnimator().SetBool("IsCharginHeavyAttack", newIsChargingHeavyAttack);
        }

        // Server RPCs are a way to send a message from the client to the server
        [ServerRpc]
        public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // If this character is the HOST/Server then play the animation for all clients in all clients rpcs
            if (IsServer)
            {
                PlayActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
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


        // ATTACK ANIMATION
        [ServerRpc]
        public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // If this character is the HOST/Server then play the animation for all clients in all clients rpcs
            if (IsServer)
            {
                PlayActionAttackAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayActionAttackAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAttackActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            characterManager.SetApplyRootMotion(applyRootMotion);
            characterManager.GetCharacterAnimator().CrossFade(animationID, 0.2f);
        }

        // DAMAGE ANIMATION
        [ServerRpc(RequireOwnership = false)]
        public void NotifyTheServerOfDamageActionAnimationServerRpc(
            ulong characterCausingDamageID,
            ulong damageCharacterID,
            float physical,
            float magic,
            float fire,
            float lightning,
            float holy,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ
        )
        {
            if (IsServer)
            {
                NotifyTheServerOfDamageActionAnimationClientRpc(
                    characterCausingDamageID,
                    damageCharacterID,
                    physical,
                    magic,
                    fire,
                    lightning,
                    holy,
                    poiseDamage,
                    angleHitFrom,
                    contactPointX,
                    contactPointY,
                    contactPointZ
                );
            }
        }

        [ClientRpc]
        public void NotifyTheServerOfDamageActionAnimationClientRpc(
            ulong characterCausingDamageID,
            ulong damageCharacterID,
            float physical,
            float magic,
            float fire,
            float lightning,
            float holy,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ
        )
        {
            ProcessCharacterDamageFromServer(
                characterCausingDamageID,
                damageCharacterID,
                physical,
                magic,
                fire,
                lightning,
                holy,
                poiseDamage,
                angleHitFrom,
                contactPointX,
                contactPointY,
                contactPointZ
            );
        }

        private void ProcessCharacterDamageFromServer(
            ulong characterCausingDamageID,
            ulong damageCharacterID,
            float physical,
            float magic,
            float fire,
            float lightning,
            float holy,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ
        )
        {
            CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damageCharacterID].gameObject.GetComponent<CharacterManager>();
            CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

            TakeDamageEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.Instance.GetTakeDamageEffect());
            takeDamageEffect.SetDamagesEffects(physical, magic, fire, lightning, holy, poiseDamage);
            takeDamageEffect.SetDamageDirection(angleHitFrom, contactPointX, contactPointY, contactPointZ);
            takeDamageEffect.SetCharacterCausingDamage(characterCausingDamage);

            damagedCharacter.GetCharacterEffectsManager().ProcessInstantEffect(takeDamageEffect);
        }
    }
}