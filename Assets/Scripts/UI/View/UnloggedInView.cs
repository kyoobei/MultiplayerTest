using Gelo.UI.Model;
using System;
using UnityEngine;
namespace Gelo.UI.View
{
    public class UnloggedInView : View
    {
        [SerializeField]
        private UnloggedInModel m_unloginModel;

        public Action<string, string> OnLogin;
        public Action<string, string> OnSignup;

        private void OnEnable()
        {
            m_unloginModel.OnClickLogin += RecievedLogin;
            m_unloginModel.OnClickSignup += RecievedSignup;
        }
        private void OnDisable()
        {
            m_unloginModel.OnClickLogin -= RecievedLogin;
            m_unloginModel.OnClickSignup -= RecievedSignup;
        }
        public void Clear() => m_unloginModel.Clear();
        private void RecievedLogin() => OnLogin?.Invoke(m_unloginModel.GetEmail(),
            m_unloginModel.GetPassword());
        private void RecievedSignup() => OnSignup?.Invoke(m_unloginModel.GetEmail(),
            m_unloginModel.GetPassword());
        public override void Activate() => m_unloginModel.gameObject.SetActive(true);
        public override void Deactivate() => m_unloginModel.gameObject.SetActive(false);
    }
}