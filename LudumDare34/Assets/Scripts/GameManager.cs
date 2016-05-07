using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    private SyncManager _syncManager;

    public SyncManager syncManager {
        get {
            if (_syncManager == null)
                _syncManager = gameObject.AddComponent<SyncManager>();
            return _syncManager;
        }
    }
    
    public void Awake() {
    }
}
