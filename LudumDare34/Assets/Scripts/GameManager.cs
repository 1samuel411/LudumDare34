using UnityEngine;
using System.Collections;
using Assets.Scripts.DataObjectLayer;
using Assets.Scripts.ThirdPartyConnection.SyncManager;

public class GameManager: MonoBehaviour, ISyncAcessor {

    #region Datasets
    public PlayerDetails playerDetails;

    #endregion

    public static GameManager instance;

    public void Awake() {
        instance = this;
    }

    public void AuthenticateAndLoad(IEnumerator action) {
        syncInitializer.AuthenticateAndLoad(InitializeDatasets(action));
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
        playerDetails = new PlayerDetails(syncInitializer.OpenOrCreateDataset("PlayerDetails"));
        yield return new WaitUntil(() => playerDetails.isFirstSyncStatus != SyncStatus.Pending);
        yield return new WaitForSeconds(1.0f);
        if(action != null)
            StartCoroutine(action);
    }
    #endregion
}
