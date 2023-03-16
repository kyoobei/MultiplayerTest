using System;
using Realms.Sync;
namespace Gelo.Realm.Auth
{
    public abstract class Auth
    {
        public string Email => m_email;
        protected User m_user;
        protected App m_app;
        protected string m_email;
        public Auth(User user, App app)
        {
            m_user = user;
            m_app = app;
        }
        // NOTE: Login is not included because different auths have diff login method
        public abstract void Logout(Action<bool, string> OnResult = null);
    }
}