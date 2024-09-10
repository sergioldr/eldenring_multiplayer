using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SL
{
    public class PlayerManagerUI : MonoBehaviour
    {
        public static PlayerManagerUI Instance { get; private set; }

        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient;

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

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;
                // Here we shutdown to be a client, because we started as a host on title screen
                NetworkManager.Singleton.Shutdown();
                // Here we start as a client
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
