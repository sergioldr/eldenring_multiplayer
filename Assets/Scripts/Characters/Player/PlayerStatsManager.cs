using UnityEngine;

namespace SL
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        private PlayerManager playerManager;

        protected override void Awake()
        {
            base.Awake();

            playerManager = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();

            PlayerNetworkManager playerNetworkManager = playerManager.GetPlayerNetworkManager();

            CalculateBaseHealthBasedOnVitalityLevel(playerNetworkManager.networkVitality.Value);
            CalculateBaseStaminaBasedOnEnduranceLevel(playerNetworkManager.networkEndurance.Value);
        }
    }
}
