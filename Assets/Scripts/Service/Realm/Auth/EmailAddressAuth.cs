using MongoDB.Bson;
using Realms.Sync;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Gelo.Realm.Auth
{
    public class EmailAddressAuth : Auth
    {
        public EmailAddressAuth(User user, App app) : base(user, app)
        {
            m_user = user;
            m_app = app;
        }
        /// <summary>
        /// Login user using email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="OnResult"></param>
        [UnityEngine.Scripting.Preserve]
        public async void Login(string email, string password, Action<bool, string> OnResult = null)
        {
            try
            {
                m_user = await m_app.LogInAsync(Credentials.EmailPassword(email, password));
                m_email = m_user.Profile.Email;
                OnResult?.Invoke(true, string.Empty);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                var splitMessage = e.Message.Split(':')[0];
                OnResult?.Invoke(false, splitMessage);
            }
        }
        /// <summary>
        /// Signup user using email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="OnResult"></param>
        [UnityEngine.Scripting.Preserve]
        public async void Signup(string email, string password, Action<bool, string> OnResult = null) 
        {
            try
            {
                await m_app.EmailPasswordAuth.RegisterUserAsync(email, password);
                OnResult?.Invoke(true, string.Empty);
            }
            catch (Exception e) 
            {
                UnityEngine.Debug.Log(e.Message);
                OnResult?.Invoke(false, e.Message);
            }
        }
        /// <summary>
        /// Send reset password using email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="OnResult"></param>
        [UnityEngine.Scripting.Preserve]
        public async void SendResetPassword(string email, Action<bool, string> OnResult = null) 
        {
            try
            {
                // await m_app.EmailPasswordAuth.SendResetPasswordEmailAsync(email);
                await m_app.EmailPasswordAuth.CallResetPasswordFunctionAsync(email, "preset");
                OnResult?.Invoke(true, string.Empty);
            }
            catch (Exception e) 
            {
                UnityEngine.Debug.Log(e.Message);
                OnResult?.Invoke(false, e.Message);
            }
        }
        /// <summary>
        /// Resend confirmation email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="OnResult"></param>
        [UnityEngine.Scripting.Preserve]
        public async void ResendConfirmationEmail(string email, Action<bool, string> OnResult = null)
        {
            try
            {
                // NOTE: disabled for a bit because it keeps sending default mongo email
                // NOTE2: just try running the email checking part since the process still the same
                // await m_app.EmailPasswordAuth.ResendConfirmationEmailAsync(email);
                IsEmailAvailable(email, (isValid, result) =>
                {
                    if (result.Contains("error"))
                    {
                        OnResult?.Invoke(false, result["error"].ToString());
                    }
                    else 
                    {
                        if (result["exist"] == false)
                        {
                            OnResult?.Invoke(false, "The email you entered is invalid or unregistered.\nPlease use the email that you signed up with, or sign up for a new account.");
                        }
                        else 
                        {
                            if (result["verified"] == false)
                            {
                                OnResult?.Invoke(true, "Successfully sent a confirmation email");
                            }
                            else
                            {
                                OnResult?.Invoke(false, "This account has already been verified. You can proceed to login.");
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                OnResult?.Invoke(false, e.Message);
            }
        }
        /// <summary>
        /// Logout current user
        /// </summary>
        /// <param name="OnResult"></param>
        [UnityEngine.Scripting.Preserve]
        public override async void Logout(Action<bool, string> OnResult = null)
        {
            try
            {
                await m_user.LogOutAsync();
                OnResult?.Invoke(true, string.Empty);
            }
            catch(Exception e)
            {
                OnResult?.Invoke(false, e.Message);
            }
        }
        /// <summary>
        /// Checks if an email is available or not. Note: if the email is not verified, 
        /// this would send another confirmation email to the target
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async void IsEmailAvailable(string email, Action<bool, BsonDocument> OnResult)
        {
            var adminTokenResult = await Server.Instance.GetAdminTokenAccess();
            if (adminTokenResult.Contains("error"))
            {
                OnResult?.Invoke(false, adminTokenResult);
            }
            else
            {
                string projectId = Server.Instance.ProjectId;
                string appId = Server.Instance.AppId;
                // Note: subject to change if the link indicated in the api also changes
                string url = $"https://realm.mongodb.com/api/admin/v3.0/groups/{projectId}/apps/{appId}/user_registrations/by_email/{email}/run_confirm";
                // Note: unity quick fix for post method
                // Reference: https://stackoverflow.com/questions/68156230/unitywebrequest-post-not-sending-body
                using var www = UnityWebRequest.Post(url, "");
                www.SetRequestHeader("Authorization", $"Bearer {adminTokenResult["access_token"]}");

                var operation = www.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                var response = www.downloadHandler.text;
                var responseVal = BsonDocument.Parse(response);
                BsonDocument returnDoc;
                if (operation.webRequest.responseCode == 202)
                {
                    // User exist but has not been verified therefore resend confirmation.
                    returnDoc = new BsonDocument()
                    {
                        { "exist", true },
                        { "verified", false }
                    };
                    OnResult?.Invoke(false, returnDoc);
                }
                else if (operation.webRequest.responseCode == 400)
                {
                    // User already confirmed or Email/Password auth not enabled
                    returnDoc = new BsonDocument()
                    {
                        { "exist", true },
                        { "verified", true }
                    };
                    OnResult?.Invoke(false, returnDoc);
                }
                else if (operation.webRequest.responseCode == 404)
                {
                    // user does not exist and verified
                    returnDoc = new BsonDocument()
                    {
                        { "exist", false },
                        { "verified", false }
                    };
                    OnResult?.Invoke(true, returnDoc);
                }
                else
                {
                    BsonDocument err = new BsonDocument
                    {
                        {"error", www.result }
                    };
                    OnResult?.Invoke(false, err);
                }
            }
        }
    }
}
