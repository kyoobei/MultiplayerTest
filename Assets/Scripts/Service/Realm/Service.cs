using Realms.Sync;
namespace Gelo.Realm
{
    public abstract class Service
    {
        protected App m_app;
        protected User m_user;
        public virtual void Initialize(App app) 
        {
            m_app = app;
            m_user = m_app.CurrentUser;
        }
    }
}
