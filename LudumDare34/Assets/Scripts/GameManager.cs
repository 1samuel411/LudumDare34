using UnityEngine;
using System.Collections;
using Amazon;

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

	private bool _bInitializedAmazon = false;

	public void Awake() {
		if(!_bInitializedAmazon)
			TriggerUnityInitializer();
	}

	public void TriggerUnityInitializer() {
		UnityInitializer.AttachToGameObject(this.gameObject);
		Debug.Log("Triggered Initializer");
		_bInitializedAmazon = true;
	}
}
