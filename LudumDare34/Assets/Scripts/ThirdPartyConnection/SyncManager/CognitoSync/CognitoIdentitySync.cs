using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Facebook.Unity;

namespace FS.SyncManager.CognitoSync {
	using FacebookSync;

	public class CognitoIdentityGameCredentials {
	    public static string IDENTITY_POOL = "us-east-1:93b8b4a5-57fe-4e2d-98ec-783016def8aa";
	    public static string PROVIDER_NAME = "BWolfGameIdentityPool";
	    public static readonly RegionEndpoint REGION = RegionEndpoint.USEast1;
	}

	/* Class will handle all in-between authentications.
	 * This means it will be the intermediator for FB, Google,
	 * and any other sort of authentication.
	 */
	public class CognitoIdentitySync {

	    public bool isGoogleAuthenticated;
	    private FacebookSync _facebookSync;

		#region CognitoAWSCredentials credentials;
		private CognitoAWSCredentials _credentials;
	    public CognitoAWSCredentials credentials {
	        get {
	            if(_credentials == null)
	                _credentials = new 
	                    CognitoAWSCredentials(
	                    CognitoIdentityGameCredentials.IDENTITY_POOL,
	                    CognitoIdentityGameCredentials.REGION);
	            return _credentials;
	        }
	    }
		#endregion

		#region CognitoSyncManager syncManager
		private CognitoSyncManager _cognitoSyncManager;
	    public CognitoSyncManager cognitoSyncManager {
	        get {
				if(_cognitoSyncManager == null)
					_cognitoSyncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig() 
	                { RegionEndpoint = CognitoIdentityGameCredentials.REGION });
				return _cognitoSyncManager;
	        }
	    }
		#endregion

	    public CognitoIdentitySync() {
	        isGoogleAuthenticated = false;
	    }

	    //The facebook sync already takes care of the initialization, and logging in, their 
	    //is no need to continue it from here. either they are logged in, or they are not.
	    //public CognitoIdentitySync() {
	        //_facebookSync = new FacebookSync();
	        
	        //if(FB.IsLoggedIn)
	        //    AddFacebookTokenToCognito();
	    //}

	    private void AddFacebookTokenToCognito() {
	        Debug.Log("Facebook is logged in. adding login to Cognito...");
	        credentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
	    }

	    public void AddGoogleTokenToCognito(string token) {
	        Debug.Log("Google is logged in, Adding Login to Cognito..." + token);
	        credentials.AddLogin("accounts.google.com", token);
	        isGoogleAuthenticated = true;
	    }

	    public void SyncDataSetTest() {
	        Dataset playerInfo;
			playerInfo = cognitoSyncManager.OpenOrCreateDataset("PlayerInformationTest");
	        playerInfo.SynchronizeOnConnectivity();
	        playerInfo.Put("test", "1234567");
	        playerInfo.SynchronizeAsync();
	    }
	}
}