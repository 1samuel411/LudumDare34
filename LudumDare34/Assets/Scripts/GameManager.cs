using UnityEngine;
using System.Collections;

public static class GameManager {

    private static SyncManager _syncManager;

    public static SyncManager syncManager {
        get {
            if (_syncManager == null)
                _syncManager = new SyncManager();
            return _syncManager;
        }
    }
}
