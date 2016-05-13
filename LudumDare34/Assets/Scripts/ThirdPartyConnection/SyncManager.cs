using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class SyncManager {

    public SyncManager() {
        var config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
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
		try {
			Social.localUser.Authenticate((bool success) => {
				_isGoogleLoggedIn = Social.localUser.authenticated;
				if(success) {
					Debug.Log("Successfully Logged in!");
					try {
						//string token = PlayGamesPlatform.Instance.GetToken();
						PlayGamesPlatform.Instance.GetIdToken((token) => {
						CognitoIdentitySync.AddGoogleTokenToCognito(token);
						CognitoIdentitySync.SyncDataSetTest(); //This is a test.
						});
					} catch (UnityException e) {
						string t = e.Message;
						Debug.Log("Error " + t);
					}
				} else
					Debug.Log("Login Failed!");
				if(callback != null)
					callback.Invoke();
			});
		} catch (Exception ex) {
			Debug.LogFormat("Error: {0}", ex.Message);
		}
    }

    public void LoginOrLogout(Action callback) {
        bool bOk = false;
        if (_isGoogleLoggedIn) {
            PlayGamesPlatform.Instance.SignOut();
            _isGoogleLoggedIn = false;
			callback.Invoke();
        } else
			GoogleAuthenticates(callback);
    }
}
