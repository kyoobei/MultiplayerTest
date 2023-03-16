using Realms.Sync;
using System.Threading.Tasks;

namespace Gelo.Realm.Function
{
    public class FunctionService : Service
    {
        public override void Initialize(App app)
        {
            base.Initialize(app);
        }
        /// <summary>
        /// Call a generic function that will return a generic class. The most generic
        /// return can be a BsonDocument or BsonValue
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="functionParameters"></param>
        public async Task<T> Call<T>(string functionName, object[] functionParameters)
        {
            return await m_app.CurrentUser.Functions.CallAsync<T>(functionName, functionParameters);
        }
    }
}
