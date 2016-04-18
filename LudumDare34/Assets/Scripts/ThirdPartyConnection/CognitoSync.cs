using UnityEngine;
using System.Collections;
using Amazon.CognitoSync.SyncManager;
using Amazon.Runtime;

public class CognitoGameSync {

    private Dataset playerInfo;
    private CognitoSyncManager _syncManager;

    public CognitoGameSync(CognitoSyncManager syncManager) {
        _syncManager = syncManager;
    }

    public void InitiateDataset(string datasetName) {
        playerInfo = _syncManager.OpenOrCreateDataset(datasetName);
        playerInfo.SynchronizeOnConnectivity();
    }
}
