using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Google.Developers;

namespace FS.SyncManager {
	using CognitoSync;

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
			PlayGamesPlatform.Instance.Authenticate((bool success) => {
				_isGoogleLoggedIn = PlayGamesPlatform.Instance.IsAuthenticated();
				if(success) {
					Debug.Log("Unity: Successfully Logged in!");
					PlayGamesPlatform.Instance.GetIdToken(GetGoogleIDToken);
				} else
					Debug.Log("Unity: Login Failed!");
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
		
	public void GetGoogleIDToken(string token) {
		Debug.Log("Unity: This is the google Token for amazon " + token);
		CognitoIdentitySync.AddGoogleTokenToCognito(token);
		CognitoIdentitySync.SyncDataSetTest(); //This is a test.
	}
}
}