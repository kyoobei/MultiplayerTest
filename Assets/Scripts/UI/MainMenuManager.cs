using UnityEngine;
using Gelo.UI.Controller;
using Gelo.Realm;
using System;

namespace Gelo.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Controllers")]
        [SerializeField]
        private LoginController m_loginController;
        [SerializeField]
        private SignupController m_signupController;
        [SerializeField]
        private LogoutController m_logoutController;
        [SerializeField]
        private DialogController m_dialogController;

        private bool m_init = false;
        
        public Action OnStartGame;
     
        private void Update()
        {
            if (Server.Instance == null)
                return;

            if (!m_init)
            {
                if (Server.Instance.Auth.CurrentUser != null) 
                {
                    m_loginController.UpdateWelcomeText(Server.Instance.Auth.Email);
                    m_loginController.ActivateLoggedView();
                }
                else 
                {
                    m_loginController.ActivateUnloggedView();
                }
                m_init = true;
            }
        }
        private void OnEnable()
        {
            m_loginController.OnStart += RecievedStartGame;
            m_logoutController.OnLogout += RecievedLogout;

            m_loginController.OnWaitStart += RecievedWaitStart;
            m_signupController.OnWaitStart += RecievedWaitStart;
            m_logoutController.OnWaitStart += RecievedWaitStart;

            m_loginController.OnWaitEnd += RecievedWaitEnd;
            m_signupController.OnWaitEnd += RecievedWaitEnd;
            m_logoutController.OnWaitEnd += RecievedWaitEnd;

            m_loginController.OnGeneralPopup += RecievedNotification;
            m_signupController.OnGeneralPopup += RecievedNotification;
            m_logoutController.OnGeneralPopup += RecievedNotification;
        }
        private void OnDisable()
        {
            m_loginController.OnStart -= RecievedStartGame;
            m_logoutController.OnLogout -= RecievedLogout;

            m_loginController.OnWaitStart -= RecievedWaitStart;
            m_signupController.OnWaitStart -= RecievedWaitStart;
            m_logoutController.OnWaitStart -= RecievedWaitStart;

            m_loginController.OnWaitEnd -= RecievedWaitEnd;
            m_signupController.OnWaitEnd -= RecievedWaitEnd;
            m_logoutController.OnWaitEnd -= RecievedWaitEnd;

            m_loginController.OnGeneralPopup -= RecievedNotification;
            m_signupController.OnGeneralPopup -= RecievedNotification;
            m_logoutController.OnGeneralPopup -= RecievedNotification;
        }
        private void RecievedStartGame()
        {
            m_loginController.DeactivateViews();
            OnStartGame?.Invoke();
        }
        private void RecievedLogout()
        {
            m_loginController.ActivateUnloggedView();
        }
        private void RecievedNotification(string header, string body) => m_dialogController.RecievedNotification(header, body);
        private void RecievedWaitStart() => m_dialogController.RecievedPleaseWaitStart();
        private void RecievedWaitEnd() => m_dialogController.RecievedPleaseWaitEnd();
    }
}
