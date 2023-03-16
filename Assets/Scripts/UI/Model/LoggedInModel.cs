using UnityEngine;
using UnityEngine.UI;
using System;
namespace Gelo.UI.Model
{
    public class LoggedInModel : Model
    {
        [SerializeField]
        private Text m_welcomeText;

        public Action OnClickStart;
        public Action OnClickLogout;

        public void ClickedStart() => OnClickStart?.Invoke();
        public void ClickedLogout() => OnClickLogout?.Invoke();

        public void UpdateWelcomeText(string value)
        {
            m_welcomeText.text = $"Welcome back {value}!";
        }
        public override void Clear()
        {
            m_welcomeText.text = string.Empty;
        }
    }
}
