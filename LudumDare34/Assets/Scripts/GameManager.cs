using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.DataObjectLayer;
using Assets.Scripts.ThirdPartyConnection.SyncManager;

public class GameManager: MonoBehaviour, ISyncAcessor {

    #region Datasets
    public PlayerDetails playerDetails;
    public OptionSettings optionSettings;
    public PlayerPurchases playerPurchases;
    #endregion

    public static GameManager instance;

    public void Awake() {
        instance = this;
    }

    public void AuthenticateAndLoad(IEnumerator action = null) {
        syncInitializer.AuthenticateAndLoad(InitializeDatasets(action));
    }

    public void AuthenticateAndLoad(Action callback) {
        syncInitializer.LoginOrLougout(InitializeDatasets(), callback);
    }

    #region ISyncAcessor Members
    private SyncInitializer _syncInitializer;
    public SyncInitializer syncInitializer {
        get {
            if(_syncInitializer == null)
                _syncInitializer = this.gameObject.AddComponent<SyncInitializer>();
            return _syncInitializer;
        }
    }

    public IEnumerator InitializeDatasets(IEnumerator action = null) {
        Debug.Log("BlazeWolf: Datasets are being Initialized");
        optionSettings = new OptionSettings(syncInitializer.OpenOrCreateDataset("OptionSettings"));
        playerPurchases = new PlayerPurchases(syncInitializer.OpenOrCreateDataset("PlayerPurchases"));
        playerDetails = new PlayerDetails(syncInitializer.OpenOrCreateDataset("PlayerDetails"));
        yield return new WaitUntil(() => playerDetails.isFirstSyncStatus != SyncStatus.Pending);
        Debug.Log("BlazeWolf: Datasets have been Initialized");
        if(action != null)
            StartCoroutine(action);
    }

    public void ResyncDatasets() {
        playerDetails.SynchronizeAndResync();
    }
    #endregion
}
