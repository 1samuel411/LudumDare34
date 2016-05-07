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
using Amazon.SecurityToken;
using Facebook.Unity;
using GooglePlayGames;

public class CognitoIdentityGameCredentials {
    public static string IDENTITY_POOL = "us-east-1:93b8b4a5-57fe-4e2d-98ec-783016def8aa";
    public static string PROVIDER_NAME = "www.blazewolf.com";
    public static readonly RegionEndpoint REGION = RegionEndpoint.USEast1;
}

/* Class will handle all in-between authentications.
 * This means it will be the intermediator for FB, Google,
 * and any other sort of authentication.
 */
public class CognitoIdentitySync {

    private FacebookSync _facebookSync;
    private CognitoAWSCredentials _credentials;
    private CognitoSyncManager _syncManager;

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

    public CognitoSyncManager syncManager {
        get {
            if(_syncManager == null)
                _syncManager = new CognitoSyncManager(credentials, new AmazonCognitoSyncConfig() 
                { RegionEndpoint = CognitoIdentityGameCredentials.REGION });
            return _syncManager;
        }
    }

    //The facebook sync already takes care of the initialization, and logging in, their 
    //is no need to continue it from here. either they are logged in, or they are not.
    public CognitoIdentitySync() {
        PlayGamesPlatform.Activate();
        //_facebookSync = new FacebookSync();
        
        //if(FB.IsLoggedIn)
        //    AddFacebookTokenToCognito();
    }

    public void GoogleAuthenticates(Action callback) {
        PlayGamesPlatform.DebugLogEnabled = true;
        Social.localUser.Authenticate((bool success) => {
            if(success) {
                Debug.Log("Successfully Logged in!");
                AddGoogleTokenToCognito();
            } else
                Debug.Log("Login Failed!");
            if(callback != null) callback.Invoke();
        });
    }

    private void AddFacebookTokenToCognito() {
        Debug.Log("Facebook is logged in. adding login to Cognito...");
        credentials.AddLogin("graph.facebook.com", AccessToken.CurrentAccessToken.TokenString);
    }

    private void AddGoogleTokenToCognito() {
        Debug.Log("Google is logged in, Adding Login to Cognito...");
        //credentials.AddLogin("Google stuff", "");
    }
}
