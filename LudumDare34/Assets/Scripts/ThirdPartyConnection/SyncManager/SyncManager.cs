using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using FS.SyncManager.CognitoSync;

namespace FS.SyncManager {

	public sealed class SyncManager {

	    public SyncManager() {
	        var config = new PlayGamesClientConfiguration.Builder().Build();
	        PlayGamesPlatform.InitializeInstance(config);
	        PlayGamesPlatform.DebugLogEnabled = true;
	        PlayGamesPlatform.Activate();
	        _isGoogleLoggedIn = false;
	    }

	    private bool _isGoogleLoggedIn;

	    public bool isGoogleLoggedIn {
	        get { return _isGoogleLoggedIn; } 
	        set { _isGoogleLoggedIn = value; }
	    }

	    #region Amazon Cognito Sync
	    private CognitoIdentitySync _cognitoIdentitySync;
	    public CognitoIdentitySync cognitoIdentitySync {
	        get { return _cognitoIdentitySync ?? (_cognitoIdentitySync = new CognitoIdentitySync()); }
	    }
	    #endregion

	    public void GoogleAuthenticates(Action callback = null) {
			try {
				PlayGamesPlatform.Instance.Authenticate((bool success) => {
					_isGoogleLoggedIn = PlayGamesPlatform.Instance.IsAuthenticated();
					if(success) {
						Debug.Log("Unity: Successfully Logged in!");
						PlayGamesPlatform.Instance.GetIdToken(cognitoIdentitySync.AddGoogleTokenToCognito);
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
	}
}