using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TerraNPCBot.DatabaseItems;
using Newtonsoft.Json;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TerraNPCBot {
    public class Database {

        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<BsonDocument> playerCollection;
        //private IMongoCollection<BsonDocument> recordedPacketCollection;  soon

        private const string USER_ID = "ID";
        private const string USER_DATA = "Data";

        

        public Database(string connexionString) {
            client = new MongoClient(connexionString);

            db = client.GetDatabase("BotsPlugin");
            playerCollection = db.GetCollection<BsonDocument>("Players");
        }

        public bool HasUserEntry(int id) {
            var filter = Builders<BsonDocument>.Filter.Eq(USER_ID, id);
            return playerCollection.Find(filter).FirstOrDefault() != null;
        }

        public void AddUserEntry(BTSPlayer user, int id) {
            BsonDocument playerDocument = GenerateUserDocument(user, id);

            playerCollection.InsertOne(playerDocument);
        }

        public void UpdateUserEntry(BTSPlayer user, int id) {
            BsonDocument playerDocument = GenerateUserDocument(user, id);

            var filterExisting = Builders<BsonDocument>.Filter.Eq(USER_ID, id);
            playerCollection.ReplaceOne(filterExisting, playerDocument);
        }

        public BTSPlayer LoadUserEntry(int id, int index) {
            var filter = Builders<BsonDocument>.Filter.Eq(USER_ID, id);

            var entry = playerCollection.Find(filter).FirstOrDefault();

            string data = entry["Data"].ToString();

            return JsonConvert.DeserializeObject<DBPlayer>(data).ConvertDBItem(index);
        }

        private BsonDocument GenerateUserDocument(BTSPlayer user, int id) {

            var playerDocument = new BsonDocument() {
                { USER_ID, id },
                { USER_DATA, JsonConvert.SerializeObject(DBPlayer.ConvertPlayer(user), Formatting.Indented) }
            };
        
            return playerDocument;
        }
    }
}
