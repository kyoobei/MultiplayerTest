using System;
using Gelo.UI.View;
using UnityEngine;
namespace Gelo.UI.Controller
{
    public class DialogController : MonoBehaviour
    {
        [SerializeField]
        private NotifyView m_notifyView;
        [SerializeField]
        private PleaseWaitView m_pleaseWaitView;

        public void RecievedNotification(string header, string body) => m_notifyView.UpdateNotification(header, body);
        public void RecievedPleaseWaitStart() => m_pleaseWaitView.Activate();
        public void RecievedPleaseWaitEnd() => m_pleaseWaitView.Deactivate();
    }
}
