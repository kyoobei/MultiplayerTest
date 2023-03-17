using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

namespace Gelo.Game
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunner m_runner;
        [SerializeField] private NetworkPrefabRef m_playerPrefab;
        private Dictionary<PlayerRef, NetworkObject> m_spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

        private void Update()
        {
            if (m_runner == null)
                return;

            // check fo runner status
        }
        public void StartGame()
        {
            StartGameMode(GameMode.AutoHostOrClient);
        }
        #region FUSION CALLBACKS
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
        {
            if (runner.IsServer)
            {
                // Create a unique position for the player
                Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
                NetworkObject networkPlayerObject = runner.Spawn(m_playerPrefab, spawnPosition, Quaternion.identity, player);
                // Keep track of the player avatars so we can remove it when they disconnect
                m_spawnedPlayers.Add(player, networkPlayerObject);
            }
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
        {
            if (m_spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                m_spawnedPlayers.Remove(player);
            }
        }
        public void OnInput(NetworkRunner runner, NetworkInput input) 
        {
            var data = new NetworkInputData();

            if (Input.GetKey(KeyCode.W))
            {
                data.Buttons.Set(InputButtons.Forward, true);
            }
            if (Input.GetKey(KeyCode.S))
            {
                data.Buttons.Set(InputButtons.Backward, true);
            }
            if (Input.GetKey(KeyCode.A))
            {
                data.Buttons.Set(InputButtons.Left, true);
            }
            if (Input.GetKey(KeyCode.D))
            {
                data.Buttons.Set(InputButtons.Right, true);
            }

            input.Set(data);
        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        #endregion
        private async void StartGameMode(GameMode mode)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            m_runner = gameObject.AddComponent<NetworkRunner>();
            m_runner.ProvideInput = true;

            // Start or join (depends on gamemode) a session with a specific name
            await m_runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestProject",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }


    }
}
