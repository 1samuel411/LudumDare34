using UnityEngine;
using System.Collections;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

public class AWSDynamoDBSync: MonoBehaviour {

    private AmazonDynamoDBClient client;

    // Use this for initialization
    public void Start() {
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(AWSDataCredentials.IDENTITY_POOL_ID, AWSDataCredentials.REGION_ENDPOINT);
        AWSCredentials awsCredentials = credentials;
        client = new AmazonDynamoDBClient(awsCredentials);
        DynamoDBContext Context = new DynamoDBContext(client);
    }

    // Update is called once per frame
    public void Update() {
    }

    public void DescribeTable() {
        string resultText = string.Empty;
        var request = new DescribeTableRequest {TableName = @"NightBearHighScores"};
        client.DescribeTableAsync(request, (result) => {
            if (result.Exception != null) {
                resultText += result.Exception.Message;
                Debug.Log(result.Exception);
                return;
            }
            var response = result.Response;
            TableDescription description = response.Table;
            resultText += ("Name: " + description.TableName + "\n");
            resultText += ("# of items: " + description.ItemCount + "\n");
            resultText += ("Provision Throughput (reads/sec): " +
                           description.ProvisionedThroughput.ReadCapacityUnits + "\n");
            resultText += ("Provision Throughput (reads/sec): " +
                                description.ProvisionedThroughput.WriteCapacityUnits + "\n");
        }, null);

        Debug.Log(resultText);
    }
}
