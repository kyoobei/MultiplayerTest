using Gelo.UI.View;
using UnityEngine;
using Gelo.Realm;
using System;

namespace Gelo.UI.Controller
{
    public class LogoutController : Controller
    {
        [SerializeField]
        private LoggedInView m_loggedInView;

        private void OnEnable() => m_loggedInView.OnLogout += RecievedLogout;
        private void OnDisable() => m_loggedInView.OnLogout -= RecievedLogout;

        public Action OnLogout;

        private void RecievedLogout()
        {
            OnWaitStart?.Invoke();
            Server.Instance.Auth.EmailPassword.Logout((isValid, result) =>
            {
                OnWaitEnd?.Invoke();
                if (!isValid)
                {
                    // general errors that we didn't catch
                    OnGeneralPopup?.Invoke("GENERAL ERROR", "There is a problem with our current server. Please try again.");
                }
                else 
                {
                    OnLogout?.Invoke();
                }
            });
        }
    }
}
