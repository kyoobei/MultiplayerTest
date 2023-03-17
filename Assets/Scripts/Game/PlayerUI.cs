using Fusion;
using Gelo.Realm;
using Gelo.Schema;
using MongoDB.Bson;
using Realms;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Gelo.Game
{
    public class PlayerUI : NetworkBehaviour
    {
        [SerializeField]
        private TMP_Text m_playerName;
        [SerializeField]
        private TMP_Text m_playerScore;
        [SerializeField]
        private Button m_plusBtn;
        [SerializeField]
        private Button m_minusBtn;

        [Networked(OnChanged = nameof(OnChangedName))]
        public NetworkString<_32> playerName { get; set; }
        [Networked(OnChanged = nameof(OnChangedScore))]
        public NetworkString<_16> playerScore { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                RPC_SetName(Server.Instance.Auth.Email);
                m_plusBtn.gameObject.SetActive(false);
                m_minusBtn.gameObject.SetActive(false);
                GetCurrentScore();
                ListenToRealm();
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }

        private static void OnChangedName(Changed<PlayerUI> changed)
        {
            changed.Behaviour.OnNameChanged();
        }
        private static void OnChangedScore(Changed<PlayerUI> changed)
        {
            changed.Behaviour.OnScoreChanged();
        }
        private void OnNameChanged() 
        {
            m_playerName.text = playerName.ToString();
        }
        private void OnScoreChanged()
        {
            m_playerScore.text = playerScore.ToString();
        }
        #region RPC Calls
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetName(string name, RpcInfo info = default)
        {
            playerName = name;
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetScore(string score, RpcInfo info = default)
        {
            playerScore = score;
        }
        #endregion
        #region UI
        public void ClickedAddScore()
        {
            AddScore();
        }
        public void ClickedMinusScore()
        {
            RemoveScore();
        }
        private async void AddScore()
        {
            object[] param = { m_playerName.text, 1 };
            var value = await Server.Instance.Function.Call<BsonDocument>("users_addScore", param);
        }
        private async void RemoveScore()
        {
            object[] param = { m_playerName.text, -1 };
            var value = await Server.Instance.Function.Call<BsonDocument>("users_addScore", param);
        }
        #endregion
        #region REALM CALLS
        private void ListenToRealm()
        {
            string player = $"player={Server.Instance.Auth.CurrentUser.Id}";
            Server.Instance.Sync.StartRealm(player, player, (isValid, result) =>
            {
                if (isValid)
                {
                    Server.Instance.Sync.ListenToRealmCollection<data_players>(player, ListeningToDataUpdate, 
                        (isListenValid, isListenResult)=> 
                        {
                            if (!isValid) 
                            {
                                Debug.LogError(isListenResult);
                            }
                        });
                }
                else 
                {
                    Debug.LogError(result);
                }
            });
        }
        private void ListeningToDataUpdate(IRealmCollection<RealmObject> arg1, ChangeSet arg2, Exception arg3)
        {
            GetCurrentScore();
        }
        private async void GetCurrentScore()
        {
            object[] param = { };
            var value = await Server.Instance.Function.Call<BsonDocument>("users_getScore", param);
            var scoreRecieved = value["score"].AsInt32;
            RPC_SetScore(scoreRecieved.ToString());
        }
        #endregion
    }
}
