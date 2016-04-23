using UnityEngine;
using System.Collections;
using Amazon;
using Amazon.CognitoSync.SyncManager;

public class CgContext {

    private string _awsIdentityPool;
    private CognitoSyncManager _syncManager;
    private RegionEndpoint _regionEndpoint;


    public string AwsIdentityPool {
        get {
            return _awsIdentityPool;
        }
    }


    public CgContext(string AWSIdentityPool) {
        _awsIdentityPool = AWSIdentityPool;
    }

    public void SynchronizeData() {
        
    }
}
