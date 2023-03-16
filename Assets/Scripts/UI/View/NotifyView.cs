using Gelo.UI.Model;
using UnityEngine;
namespace Gelo.UI.View
{
    public class NotifyView : MonoBehaviour
    {
        [SerializeField]
        private NotifyModel m_notifyModel;

        public void UpdateNotification(string header, string body)
        {
            m_notifyModel.gameObject.SetActive(true);
            m_notifyModel.UpdateNotification(header, body);
        }
    }
}
