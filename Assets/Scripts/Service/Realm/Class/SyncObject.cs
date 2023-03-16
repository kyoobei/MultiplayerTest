using Realms.Sync;
using Realms;
using System;
using UnityEngine;
namespace Gelo.Realm.Sync
{
    public class SyncObject : MonoBehaviour
    {
        public Realms.Realm Realm => m_realm;

        private Realms.Realm m_realm;
        private PartitionSyncConfiguration m_partitionSyncConfig;
        private IDisposable m_collectionToken;

        public Action<object, EventArgs> OnRecievedRealmNotification;
        public Action<IRealmCollection<RealmObject>, ChangeSet, Exception> OnRecievedCollectionNotification;

        #region PUBLIC METHODS
        public async void Initialize(string realmsPartition, User user,
            Action<bool, string> OnResult = null)
        {
            try
            {
                if (m_realm != null)
                {
                    m_realm.RealmChanged -= RecievedRealmNotification;
                }
                m_partitionSyncConfig = new PartitionSyncConfiguration(realmsPartition, user);
                m_realm = await Realms.Realm.GetInstanceAsync(m_partitionSyncConfig);
                //register to the realm notification
                m_realm.RealmChanged += RecievedRealmNotification;

                OnResult?.Invoke(true, "On Success to Initialize");
            }
            catch (Exception e)
            {
                string errorMessage = string.Empty;
                if (e.InnerException != null)
                {
                    errorMessage = string.Format($"Message: {e.Message} | Inner Exception: {e.InnerException}");
                    OnResult?.Invoke(false, errorMessage);
                }
                else
                {
                    errorMessage = e.Message;
                    OnResult?.Invoke(false, errorMessage);
                }
            }
        }
        public void ObserveCollection<T>(Action<bool, string> OnResult = null) where T : RealmObject
        {
            try
            {
                m_collectionToken = m_realm.All<T>().SubscribeForNotifications(NotificationCallbackDelegate);
                OnResult?.Invoke(true, "Success");
            }
            catch (Exception e)
            {
                OnResult?.Invoke(false, e.Message);
            }
        }
        public void DisposeRealm()
        {
            if (m_realm != null)
            {
                m_realm.RealmChanged -= RecievedRealmNotification;
            }
            m_realm?.Dispose();
        }
        public void DisposeCollectionToken(Action<bool, string> OnResult)
        {
            try
            {
                m_collectionToken.Dispose();
                OnResult?.Invoke(true, "Success token dispose");
            }
            catch (Exception e)
            {
                OnResult?.Invoke(false, e.Message);
            }
        }
        #endregion

        #region PRIVATE METHODS
        private void RecievedRealmNotification(object sender, EventArgs eventArgs)
        {
            OnRecievedRealmNotification?.Invoke(sender, eventArgs);
        }
        private void NotificationCallbackDelegate(IRealmCollection<RealmObject> sender, ChangeSet changes, Exception error)
        {
            OnRecievedCollectionNotification?.Invoke(sender, changes, error);
        }
        #endregion
    }
}
