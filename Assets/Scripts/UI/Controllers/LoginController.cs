using Gelo.UI.View;
using UnityEngine;
using System;
using Gelo.Realm;
using System.Text.RegularExpressions;

namespace Gelo.UI.Controller
{
    public class LoginController : Controller
    {
        [SerializeField]
        private LoggedInView m_loggedInView;
        [SerializeField]
        private UnloggedInView m_unloggedInView;

        public Action OnStart;

        private void OnEnable()
        {
            m_unloggedInView.OnLogin += RecievedLogin;
            m_loggedInView.OnStart += RecievedStart;
        }
        private void OnDisable()
        {
            m_unloggedInView.OnLogin -= RecievedLogin;
            m_loggedInView.OnStart -= RecievedStart;
        }
        public void UpdateWelcomeText(string email) => m_loggedInView.UpdateWelcomeText(email);
        public void ActivateLoggedView()
        {
            m_loggedInView.Activate();
            m_unloggedInView.Deactivate();
        }
        public void ActivateUnloggedView()
        {
            m_loggedInView.Deactivate();
            m_unloggedInView.Activate();
        }
        public void DeactivateViews()
        {
            m_loggedInView.Deactivate();
            m_unloggedInView.Deactivate();
        }
        private void RecievedLogin(string email, string password)
        {
            OnWaitStart?.Invoke();
            // to make sure email are in lower case and no empty space
            string modifiedEmail = email.ToLower().Trim();
            if (!IsEmailFormatCorrect(modifiedEmail))
            {
                OnWaitEnd?.Invoke();
                OnGeneralPopup?.Invoke("INVALID EMAIL FORMAT", "The email format is invalid.");
                return;
            }
            Server.Instance.Auth.EmailPassword.Login(modifiedEmail, password, (isValid, result) =>
            {
                OnWaitEnd?.Invoke();
                if (!isValid)
                {
                    if (result == "AuthError")
                    {
                        // resend confirmation mail to the user
                        Server.Instance.Auth.EmailPassword.ResendConfirmationEmail(email);
                        OnGeneralPopup?.Invoke("EMAIL NOT VERIFIED", "We have resent a confirmation email to your account. Please verify your email");
                    }
                    else if (result == "InvalidPassword")
                    {
                        OnGeneralPopup?.Invoke("INVALID PASSWORD", "The password you have given is incorrect. Please try again.");
                    }
                    else
                    {
                        // general errors that we didn't catch
                        OnGeneralPopup?.Invoke("GENERAL ERROR", "There is a problem with our current server. Please try again.");
                    }
                }
                else
                {
                    m_loggedInView.Clear();
                    OnStart?.Invoke();
                }
            });
        }
        private bool IsEmailFormatCorrect(string email)
        {
            Regex r = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return r.Match(email).Success;
        }
        private void RecievedStart() => OnStart?.Invoke();        
    }
}
