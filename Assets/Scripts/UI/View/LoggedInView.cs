using UnityEngine;
using Gelo.UI.Model;
using System;
namespace Gelo.UI.View
{
    public class LoggedInView : View
    {
        [SerializeField]
        private LoggedInModel m_loginModel;
        
        public Action OnStart;
        public Action OnLogout;

        private void Awake()
        {
            m_loginModel.OnClickStart += RecievedStart;
            m_loginModel.OnClickLogout += RecievedLogout;
        }
        private void OnDisable()
        {
            m_loginModel.OnClickStart -= RecievedStart;
            m_loginModel.OnClickLogout -= RecievedLogout;
        }
        public void UpdateWelcomeText(string email) => m_loginModel.UpdateWelcomeText(email);
        public void Clear() => m_loginModel.Clear();
        private void RecievedStart() => OnStart?.Invoke();
        private void RecievedLogout() => OnLogout?.Invoke();
        public override void Activate() => m_loginModel.gameObject.SetActive(true);
        public override void Deactivate() => m_loginModel.gameObject.SetActive(false);
    }
}
