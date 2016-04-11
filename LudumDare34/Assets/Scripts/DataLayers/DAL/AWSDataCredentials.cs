using UnityEngine;
using System.Collections;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using Amazon.Runtime;

public class AWSDataCredentials {

    public static string IDENTITY_POOL_ID = "us-east-1:50893fe8-8071-4143-893c-04974e42f0d9";
    public static RegionEndpoint REGION_ENDPOINT = RegionEndpoint.USEast1;
    public static string REGION { get { return REGION_ENDPOINT.SystemName; } }

    #region AwsCredentials
    private AWSCredentials _awsCredentials;
    public AWSCredentials AwsCredentials {
        get {
            if (_awsCredentials == null)
                _awsCredentials = this.AwsCognitoCredentials;
            return _awsCredentials;
        }
    }
    #endregion

    #region AwsCognitoCredentials
    private CognitoAWSCredentials _awsCognitoCredentials;
    public CognitoAWSCredentials AwsCognitoCredentials {
        get {
            if(_awsCognitoCredentials == null)
                _awsCognitoCredentials = new CognitoAWSCredentials(IDENTITY_POOL_ID, REGION_ENDPOINT);
            return _awsCognitoCredentials;
        }
    }
    #endregion

    #region CognitoSyncManager
    private CognitoSyncManager _cognitoSyncManager;
    public CognitoSyncManager CognitoSyncManager {
        get {
            if(_cognitoSyncManager == null)
                _cognitoSyncManager = new CognitoSyncManager(AwsCognitoCredentials, new AmazonCognitoSyncConfig { RegionEndpoint = REGION_ENDPOINT});
            return _cognitoSyncManager;
        }
    }
    #endregion

    public void AWSCognitoCredentials() {
        
    }
}
