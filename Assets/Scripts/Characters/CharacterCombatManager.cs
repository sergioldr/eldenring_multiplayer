using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class CharacterCombatManager : MonoBehaviour
    {
        private CharacterManager characterManager;

        [Header("Attack Target")]
        public CharacterManager currentTarget;

        [Header("Attack Type")]
        public AttackType currentAttackType;

        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (characterManager.IsOwner)
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    characterManager.GetCharacterNetworkManager().currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else
                {
                    currentTarget = null;
                }
            }
        }
    }
}
