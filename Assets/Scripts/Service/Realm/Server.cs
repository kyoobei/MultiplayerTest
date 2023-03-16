using System.Collections.Generic;
using UnityEngine;
using Gelo.Realm.Auth;
using Gelo.Realm.Function;
using Realms.Sync;
using UnityEngine.Networking;
using System.Threading.Tasks;
using MongoDB.Bson;
using System;

namespace Gelo.Realm
{
    public class Server : MonoBehaviour
    {
        public static Server Instance => m_instance;
        private static Server m_instance;
        public string CurrentAppId => m_currentAppId;
        public string ProjectId => m_projectId;
        public string AppId => m_appId;
        public string PublicApiKey => m_publicApiKey;
        public string PrivateApiKey => m_privateApiKey;
        [SerializeField]
        [Header("The selected Realm App will only be initialize on Start method")]
        private List<RealmConfig> m_realmApp = new List<RealmConfig>();

        [Header("Admin Keys. Ask the backend for it.")]
        [SerializeField]
        private string m_projectId;
        [SerializeField]
        private string m_publicApiKey;
        [SerializeField]
        private string m_privateApiKey;

        public AuthService Auth;
        public FunctionService Function;
        public SyncService Sync;

        public App m_app;
        private List<Service> m_services = new List<Service>();
        private string m_currentAppId;
        private string m_appId;

        public Action OnRecievedPauseState;
        public Action OnRecievedResumeState;
        public Action<bool> OnRecievedExitState;

        private void Awake()
        {
            if (m_instance == null) 
            {
                m_instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void Start()
        {
            // initialize service list
            m_services.Add(Auth = new AuthService());
            m_services.Add(Function = new FunctionService());
            m_services.Add(Sync = new SyncService());

            // to set the transform on the server prefab itself
            Sync.SetParent(this.transform);

            if (m_realmApp.Count <= 0)
            {
                Debug.LogError("No Realm Environments detected");
                return;
            }

            for (int i = 0; i < m_realmApp.Count; i++) 
            {
                if (m_realmApp[i].IsActive) 
                {
                    m_currentAppId = m_realmApp[i].Value;
                    m_appId = m_realmApp[i].AppId;

                    m_app = App.Create(m_currentAppId);
                    break;
                }
            }

            // initialize service
            m_services.ForEach(x => x.Initialize(m_app));
        }
        public async Task<BsonDocument> GetAdminTokenAccess()
        {
            // Note: as alternative, can use a class to convert json to string using JsonUtility class
            BsonDocument doc = new BsonDocument
            {
                {"username", m_publicApiKey },
                {"apiKey", m_privateApiKey }
            };
            // Note: subject to change if the link indicated in the api also changes
            string url = $"https://realm.mongodb.com/api/admin/v3.0/auth/providers/mongodb-cloud/login";
            // Note: unity quick fix for post method
            // Reference: https://stackoverflow.com/questions/68156230/unitywebrequest-post-not-sending-body
            using var www = UnityWebRequest.Put(url, doc.ToString());
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");

            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            var response = www.downloadHandler.text;

            if (www.result != UnityWebRequest.Result.Success)
            {
                BsonDocument err = new BsonDocument
                {
                    {"error", www.result }
                };
                return err;
            }
            else
            {
                var tokenVal = BsonDocument.Parse(response);
                BsonDocument success = new BsonDocument
                {
                    {"access_token", tokenVal["access_token"]}
                };
                return success;
            }
        }
        private void OnDisable()
        {
            if (Sync != null)
            {
                Sync.DisposeAllRealm();
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Debug.Log("The current app is in pause state");
                OnRecievedPauseState?.Invoke();
            }
            else
            {
                Debug.Log("The current app is in resume state");
                OnRecievedResumeState?.Invoke();
            }
        }
        private static bool WantsToQuit()
        {
            // it means that the player has already logged in and its still signed in
            Server.Instance.OnRecievedExitState?.Invoke(true);
            return true;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.wantsToQuit += WantsToQuit;
        }
    }
}
