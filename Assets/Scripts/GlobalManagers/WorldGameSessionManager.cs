using System.Collections.Generic;
using UnityEngine;

namespace SL
{
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager Instance;

        [Header("Active Players In Session")]
        public List<PlayerManager> activePlayers { get; private set; } = new List<PlayerManager>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddPlayerToActivePlayerList(PlayerManager player)
        {
            if (!activePlayers.Contains(player))
            {
                activePlayers.Add(player);
            }

            CheckAndRemoveNullPlayers();
        }

        public void RemovePlayerFromActivePlayerList(PlayerManager player)
        {
            if (activePlayers.Contains(player))
            {
                activePlayers.Remove(player);
            }

            CheckAndRemoveNullPlayers();
        }

        private void CheckAndRemoveNullPlayers()
        {
            for (int i = activePlayers.Count - 1; i > -1; i--)
            {
                if (activePlayers[i] == null)
                {
                    activePlayers.RemoveAt(i);
                }
            }
        }
    }
}
