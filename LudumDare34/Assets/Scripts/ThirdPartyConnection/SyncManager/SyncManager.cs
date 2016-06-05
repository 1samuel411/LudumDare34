using System;
using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;

namespace FS.SyncManager {
	public class SyncManager {

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

        #region CognitoAWSCredentials credentials;
        private static CognitoAWSCredentials _credentials;
        public static CognitoAWSCredentials credentials {
            get {
                if (_credentials == null) {
                    Debug.Log("Pool ID: " + CognitoGameCredentials.IDENTITY_POOL);
                    _credentials = new
                        CognitoAWSCredentials(
                        CognitoGameCredentials.IDENTITY_POOL,
                        CognitoGameCredentials.REGION);
                }
                return _credentials;
            }
        }
        #endregion

        #region CognitoSyncManager syncManager
        private CognitoSyncManager _cognitoSyncManager;
        public CognitoSyncManager cognitoSyncManager {
            get {
                if(_cognitoSyncManager == null)
                    _cognitoSyncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig() { RegionEndpoint = CognitoGameCredentials.REGION });
                return _cognitoSyncManager;
            }
        }
        #endregion

	    public void GoogleAuthenticates(Action callback = null) {
            try {
                PlayGamesPlatform.Instance.SignOut(); //guarantee account is signed out first.
                PlayGamesPlatform.Instance.Authenticate((bool success) => {
                    _isGoogleLoggedIn = PlayGamesPlatform.Instance.IsAuthenticated();
                    if(success) {
                        Debug.Log("Unity: Successfully Logged in!");
                        PlayGamesPlatform.Instance.GetIdToken((string TokenClient) => {
                            Debug.Log("Google is logged in, Adding Login to Cognito..." + TokenClient);
                            credentials.AddLogin("accounts.google.com", TokenClient);
                        });
                    } else
                        Debug.Log("Unity: Login Failed!");
                    if(callback != null)
                        callback.Invoke();
                });
            } catch(Exception ex) {
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