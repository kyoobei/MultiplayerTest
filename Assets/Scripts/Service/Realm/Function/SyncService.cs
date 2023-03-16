using Realms.Sync;
using Realms;
using System.Collections.Generic;
using System;
using UnityEngine;
using Gelo.Realm.Sync;
namespace Gelo.Realm.Function
{
    [UnityEngine.Scripting.Preserve]
    public class SyncService : Service
    {
        public List<string> AvailableCodes => availableRealmCodes;
        private List<string> availableRealmCodes = new List<string>();

        private Dictionary<string, SyncObject> m_syncObjectDict = new Dictionary<string, SyncObject>();

        Transform m_parentSpawn;

        public override void Initialize(App app)
        {
            base.Initialize(app);
        }
        public void SetParent(Transform setParent)
        {
            m_parentSpawn = setParent;
        }

        #region PUBLIC REALM METHODS
        [UnityEngine.Scripting.Preserve]
     
        public void StartRealm(string uniqueName, string partition,
            Action<bool, string> OnResult = null)
        {
            if (m_syncObjectDict.ContainsKey(uniqueName))
            {
                Debug.LogError($"This {uniqueName} code for realm partition is already created.");
                OnResult?.Invoke(false, $"This {uniqueName} code for realm partition is already created.");
            }
            else 
            {
                GameObject go = new GameObject(uniqueName);
                go.transform.SetParent(m_parentSpawn);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                var value = go.AddComponent<SyncObject>();
                value.Initialize(partition, m_app.CurrentUser, OnResult);

                Debug.Log($"The {uniqueName} code for realm has been created.");

                m_syncObjectDict.Add(uniqueName, value);
                availableRealmCodes.Add(uniqueName);
            }
        }
        [UnityEngine.Scripting.Preserve]
        public void DisposeRealm(string uniqueName, Action<bool, string> OnResult = null)
        {
            if (!m_syncObjectDict.ContainsKey(uniqueName))
            {
                Debug.LogError($"This {uniqueName} code for realm is not available");
                OnResult?.Invoke(false, $"This {uniqueName} code for realm is not available");
            }
            else 
            {
                m_syncObjectDict[uniqueName].DisposeRealm();
                var value = m_syncObjectDict[uniqueName].gameObject;
                MonoBehaviour.Destroy(value.gameObject);
                m_syncObjectDict.Remove(uniqueName);

                Debug.Log($"Successfully removed {uniqueName}");
                OnResult?.Invoke(true, $"Successfully removed {uniqueName}");
            }
        }
        [UnityEngine.Scripting.Preserve]
        public SyncObject GetRealm(string realmName)
        {
            return m_syncObjectDict[realmName];
        }
        [UnityEngine.Scripting.Preserve]
        public void DisposeAllRealm(Action<bool, string> OnResult = null)
        {
            if (m_syncObjectDict.Count <= 0)
            {
                OnResult?.Invoke(false, "No Realm initialized");
                Debug.Log("No realm initialized");
                return;
            }
            try
            {
                foreach (var realms in m_syncObjectDict)
                {
                    realms.Value.DisposeRealm();
                }
                while (m_parentSpawn.childCount > 0)
                {
                    MonoBehaviour.DestroyImmediate(m_parentSpawn.GetChild(0).gameObject);
                }
                m_syncObjectDict.Clear();
                availableRealmCodes.Clear();

                Debug.Log("Successfully dispose all realms");

                OnResult?.Invoke(true, "Success");
            }
            catch (Exception e)
            {
                OnResult?.Invoke(false, e.Message);
            }
        }
        [UnityEngine.Scripting.Preserve]
        public void ListenToRealm(string uniqueCode, Action<object, EventArgs> OnRegistered,
            Action<bool, string> OnResult = null)
        {
            try
            {
                m_syncObjectDict[uniqueCode].OnRecievedRealmNotification += OnRegistered;
                OnResult?.Invoke(true, "Success");
            }
            catch (Exception e)
            {
                OnResult?.Invoke(false, e.Message);
            }
        }
        [UnityEngine.Scripting.Preserve]
        public void ListenToRealmCollection<T>(string uniqueCode, Action<IRealmCollection<RealmObject>, ChangeSet, Exception> OnRegistered,
            Action<bool, string> OnResult = null) where T : RealmObject
        {
            m_syncObjectDict[uniqueCode].ObserveCollection<T>((isValid, result) => {
                if (isValid)
                {
                    m_syncObjectDict[uniqueCode].OnRecievedCollectionNotification += OnRegistered;
                }
                OnResult?.Invoke(isValid, result);
            });
        }
        [UnityEngine.Scripting.Preserve]
        public void RemoveListenerToRealm(string uniqueCode, Action<object, EventArgs> OnUnregister,
            Action<bool, string> OnResult = null)
        {
            try
            {
                m_syncObjectDict[uniqueCode].OnRecievedRealmNotification += OnUnregister;
                OnResult?.Invoke(true, "Success");
            }
            catch (Exception e)
            {
                OnResult?.Invoke(false, e.Message);
            }
        }
        [UnityEngine.Scripting.Preserve]
        public void RemoveListenerToCollection(string uniqueCode, Action<IRealmCollection<RealmObject>, ChangeSet, Exception> OnUnRegistered,
            Action<bool, string> OnResult = null)
        {
            if (!m_syncObjectDict.ContainsKey(uniqueCode))
            {
                OnResult?.Invoke(false, $"{uniqueCode} key value has not been found");
                return;
            }

            if (m_syncObjectDict[uniqueCode] != null)
            {
                m_syncObjectDict[uniqueCode].OnRecievedCollectionNotification -= OnUnRegistered;

                m_syncObjectDict[uniqueCode].DisposeCollectionToken((isValid, result) =>
                {
                    OnResult?.Invoke(isValid, result);
                });
            }
        }
        #endregion
    }
}
