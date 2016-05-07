using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using GooglePlayGames;

public class SyncManager {

    public SyncManager() {
        _isGoogleLoggedIn = false;
    }

    private bool _isGoogleLoggedIn;
    public bool isGoogleLoggedIn { get { return _isGoogleLoggedIn; } }

    #region Amazon Cognito Sync
    private CognitoIdentitySync _cognitoIdentitySync;
    public CognitoIdentitySync CognitoIdentitySync {
        get { return _cognitoIdentitySync ?? (_cognitoIdentitySync = new CognitoIdentitySync()); }
    }
    #endregion

    public void GoogleAuthenticates(Action callback = null) {
        PlayGamesPlatform.DebugLogEnabled = true;
        Social.localUser.Authenticate((bool success) => {
            if(success) {
                Debug.Log("Successfully Logged in!");
                CognitoIdentitySync.AddGoogleTokenToCognito("");
            } else
                Debug.Log("Login Failed!");
            _isGoogleLoggedIn = success;
            if(callback != null)
                callback.Invoke();
        });
    }

    public void LoginOrLogout(Action callback) {
        bool bOk = false;
        if (_isGoogleLoggedIn) {
            PlayGamesPlatform.Instance.SignOut();
            _isGoogleLoggedIn = false;
        } else
            GoogleAuthenticates();
        callback.Invoke();
    }
}
