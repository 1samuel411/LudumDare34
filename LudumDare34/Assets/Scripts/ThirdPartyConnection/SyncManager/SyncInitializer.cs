using System;
using UnityEngine;
using System.Collections;
using Amazon;
using Amazon.CognitoSync.SyncManager;
using FS.SyncManager;
using FS.SyncManager.CognitoSync;
#if UNITY_ANDROID || UNITY_IOS
using GooglePlayGames;
#endif

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

    public void AuthenticateAndLoad(IEnumerator action, Action callback = null) {
        syncManager.GoogleAuthenticates(callback);
        StartCoroutine(LoadDatasets(action));
    }

    private IEnumerator LoadDatasets(IEnumerator action) {
        float waitTime = 0.0f;
        //Wait Until google is authenticated or time runs out.
        yield return new WaitUntil(() => {
            bool bOk = syncManager.isGoogleLoggedIn;
            if (waitTime <= 5.0f)
                waitTime += Time.deltaTime;
            else
                bOk = true;
            return bOk;
        });
        Debug.Log("triggered loadDatasets");
        if(action != null)
            StartCoroutine(action);
    }

    public void LoginOrLougout(IEnumerator action, Action callback) {
        bool bOk = false;
        if (syncManager.isGoogleLoggedIn) {
#if UNITY_ANDROID || UNITY_IOS
            PlayGamesPlatform.Instance.SignOut();
#endif
            syncManager.isGoogleLoggedIn = false;
            callback.Invoke();
        } else
            AuthenticateAndLoad(action, callback);
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
        Dataset ds = syncManager.cognitoSyncManager.OpenOrCreateDataset(dataSetName);
        if(handlers == null) {
            handlers = new FSDatasetHandler();
        }
        ds = handlers.InitializeDataset(ds);
        return ds;
    }
}
