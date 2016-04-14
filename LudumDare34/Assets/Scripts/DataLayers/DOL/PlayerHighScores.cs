using UnityEngine;
using System.Collections;
using Amazon.DynamoDBv2.DataModel;

namespace NightBear.DOL {
    [DynamoDBTable("NightBearHighScore")]
    public class PlayerHighScores 
    {
        [DynamoDBHashKey] 
        public int Id { get; set; }
        [DynamoDBProperty]
        public string User { get; set; }
        [DynamoDBProperty]
        public string Score { get; set; }
    }
}
