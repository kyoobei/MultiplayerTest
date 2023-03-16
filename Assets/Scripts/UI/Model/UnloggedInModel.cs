using UnityEngine.UI;
using UnityEngine;
using System;
namespace Gelo.UI.Model
{
    public class UnloggedInModel : Model
    {
        [SerializeField]
        private InputField m_email;
        [SerializeField]
        private InputField m_password;

        public Action OnClickLogin;
        public Action OnClickSignup;

        public string GetEmail() => m_email.text;
        public string GetPassword() => m_password.text;
        public void ClickedLogin() => OnClickLogin?.Invoke();
        public void ClickedSignup() => OnClickSignup?.Invoke();
        public override void Clear()
        {
            m_email.text = string.Empty;
            m_password.text = string.Empty;
        }
    }
}
