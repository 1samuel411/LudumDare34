using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public SyncManager syncManager;
    
    public void Awake() {
        Initialize();
    }

    private void Initialize() {
        if(syncManager == null)
            syncManager = new SyncManager();
    }
}
