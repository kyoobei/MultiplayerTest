using Realms;
using MongoDB.Bson;
namespace Gelo.Schema
{
    public class data_players : RealmObject
    {
        [MapTo("_id")]
        [PrimaryKey]
        public ObjectId? Id { get; set; }

        [MapTo("email")]
        public string Email { get; set; }

        [MapTo("partition")]
        public string Partition { get; set; }

        [MapTo("score")]
        public int? Score { get; set; }

        [MapTo("user_id")]
        public string UserId { get; set; }
    }
}