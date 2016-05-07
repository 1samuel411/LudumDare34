using UnityEngine;
using System.Collections;

public class SyncManager : MonoBehaviour {

    public static SyncManager syncManager;

    #region Amazon Cognito Sync
    private CognitoIdentitySync _cognitoIdentitySync;
    public CognitoIdentitySync CognitoIdentitySync {
        get { return _cognitoIdentitySync ?? (_cognitoIdentitySync = new CognitoIdentitySync()); }
    }
    #endregion

    void Awake() {
        if(syncManager == null) {
            syncManager = this;
        }
    }
}
