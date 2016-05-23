using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using Amazon;
using Amazon.CognitoSync.SyncManager;
using FS.SyncManager;
using FS.SyncManager.CognitoSync;
using Assets.Scripts.ThirdPartyConnection.SyncManager;

public class SyncInitializer : MonoBehaviour {

    private SyncManager _syncManager;
    public SyncManager syncManager {
        get {
            if(_syncManager == null) {
                _syncManager = new SyncManager();
            }
            return _syncManager;
        }
    }

    public void Awake() {
        UnityInitializer.AttachToGameObject(gameObject);
    }

    public void AuthenticateAndLoad(IEnumerator action) {
        syncManager.GoogleAuthenticates();
        StartCoroutine(LoadDatasets(action));
    }

    public IEnumerator LoadDatasets(IEnumerator action) {
        float waitTime = 0.0f;
        //Wait Until google is authenticated or time runs out.
        yield return new WaitUntil(() => {
            bool bOk = syncManager.cognitoIdentitySync.isGoogleAuthenticated;
            if (waitTime <= 10.0f)
                waitTime += Time.deltaTime;
            else
                bOk = true;
            return bOk;
        });
        Debug.Log("triggered loadDatasets");
        if(action != null)
            StartCoroutine(action);
    }

    /// <summary>
    /// Opens/Creates the dataset from the Amazon Servers.
    /// </summary>
    /// <returns>The or create dataset.</returns>
    /// <param name="dataSetName">The name of the dataset</param>
    /// <param name="handlers">The handlers to use for the dataset when conflicts arise. The recommended method is to use FSDatasetHandler 
    /// as the defaults are already set. If you need to make your own handlers, or customize them, 
    /// it is best practice to inherit, and override the FSDatasetHandler.</param>
    public Dataset OpenOrCreateDataset(string dataSetName, FSDatasetHandler handlers = null) {
        Dataset ds = syncManager.cognitoIdentitySync.cognitoSyncManager.OpenOrCreateDataset(dataSetName);
        if(handlers == null) {
            handlers = new FSDatasetHandler();
        }
        ds = handlers.InitializeDataset(ds);
        return ds;
    }
}
