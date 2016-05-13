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
    private CognitoAWSCredentials _credentials;
    private CognitoSyncManager _syncManager;

    private string _AWSIdentityID;
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
        credentials.GetIdentityIdAsync(delegate(AmazonCognitoIdentityResult<string> result)
        {
            if (result.Exception != null) {
                 //Exception.
            }
            _AWSIdentityID = result.Response;
        });
    }

    public void SyncDataSetTest() {
        Dataset playerInfo;
        playerInfo = _syncManager.OpenOrCreateDataset("PlayerInformationTest");
        playerInfo.SynchronizeOnConnectivity();
        playerInfo.Put("test", "1234567");
        playerInfo.SynchronizeAsync();
    }
}
