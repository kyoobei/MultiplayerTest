using Realms.Sync;
namespace Gelo.Realm.Auth
{
    public class AuthService : Service
    {
        public User CurrentUser => m_app.CurrentUser;
        public string Email => m_app.CurrentUser.Profile.Email;

        // add auths here
        public EmailAddressAuth EmailPassword;

        public override void Initialize(App app)
        {
            base.Initialize(app);
            // initialize other auths
            EmailPassword = new EmailAddressAuth(CurrentUser, m_app);
        }
    }
}
