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
		private CognitoSyncManager _syncManager;
	    public CognitoSyncManager syncManager {
	        get {
	            if(_syncManager == null)
	                _syncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig() 
	                { RegionEndpoint = CognitoIdentityGameCredentials.REGION });
	            return _syncManager;
	        }
	    }
		#endregion

	    //The facebook sync already takes care of the initialization, and logging in, their 
	    //is no need to continue it from here. either they are logged in, or they are not.
	    public CognitoIdentitySync() {
	        //_facebookSync = new FacebookSync();
	        
	        //if(FB.IsLoggedIn)
	        //    AddFacebookTokenToCognito();
	    }

	    private void AddFacebookTokenToCognito() {
	        Debug.Log("Facebook is logged in. adding login to Cognito...");
	        credentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
	    }

	    public void AddGoogleTokenToCognito(string token) {
	        Debug.Log("Google is logged in, Adding Login to Cognito..." + token);
	        credentials.AddLogin("accounts.google.com", token);
	    }

	    public void SyncDataSetTest() {
	        Dataset playerInfo;
	        playerInfo = _syncManager.OpenOrCreateDataset("PlayerInformationTest");
	        playerInfo.SynchronizeOnConnectivity();
	        playerInfo.Put("test", "1234567");
	        playerInfo.SynchronizeAsync();
	    }
	}
}