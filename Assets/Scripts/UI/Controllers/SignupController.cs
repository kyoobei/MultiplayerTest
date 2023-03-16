using Gelo.UI.View;
using UnityEngine;
using Gelo.Realm;
using System.Text.RegularExpressions;

namespace Gelo.UI.Controller
{
    public class SignupController : Controller
    {
        [SerializeField]
        private UnloggedInView m_unloggedInView;

        private void OnEnable() => m_unloggedInView.OnSignup += VerifySignup;
        private void OnDisable() => m_unloggedInView.OnSignup -= VerifySignup;
        
        private void VerifySignup(string email, string password)
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
            Server.Instance.Auth.EmailPassword.IsEmailAvailable(modifiedEmail, (isValid, result) =>
            {
                if (!isValid)
                {
                    OnWaitEnd?.Invoke();
                    if (result.Contains("error"))
                    {
                        // to catch other errors
                        OnGeneralPopup?.Invoke("GENERAL ERROR", "There is a problem with our current server. Please try again.");
                        
                    }
                    else
                    {
                        // the email is registered previously
                        OnGeneralPopup?.Invoke("USED EMAIL", "This account has been used before. Please use another account.");
                    }
                }
                else
                {
                    ContinueToSignup(modifiedEmail, password);
                }
            });
        }
        private void ContinueToSignup(string email, string password)
        {
            Server.Instance.Auth.EmailPassword.Signup(email, password, (isValid, result) =>
            {
                OnWaitEnd?.Invoke();
                if (isValid)
                {
                    OnGeneralPopup?.Invoke("SIGNUP SUCCESSFUL", "Please check your inbox for an email confirmation to continue to the game.");
                    m_unloggedInView.Clear();
                }
                else
                {
                    // to catch other errors
                    OnGeneralPopup?.Invoke("GENERAL ERROR", "There is a problem with our current server. Please try again.");
                }
            });
        }
        private bool IsEmailFormatCorrect(string email)
        {
            Regex r = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return r.Match(email).Success;
        }
    }
}
