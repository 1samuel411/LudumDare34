using UnityEngine;
using System.Collections;
using Amazon;
using GooglePlayGames;
using FS.SyncManager;

public class GameManager: MonoBehaviour {

    private SyncManager _syncManager;
    public SyncManager syncManager {
        get {
			if (_syncManager == null) {
				_syncManager = new SyncManager();
				TriggerUnityInitializer();
			}
            return _syncManager;
        }
    }

	public static bool bInitializedAmazon = false;

	public void Awake() {
		if(!bInitializedAmazon)
			TriggerUnityInitializer();
	}

	public void TriggerUnityInitializer() {
		UnityInitializer.AttachToGameObject(this.gameObject);
		Debug.Log("Triggered Initializer");
		bInitializedAmazon = true;
	}
}
