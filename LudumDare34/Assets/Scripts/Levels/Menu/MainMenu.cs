using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class MainMenu : MonoBehaviour {

    public string IdentityPoolId = "us-east-1:50893fe8-8071-4143-893c-04974e42f0d9";

    public string Region = RegionEndpoint.USEast1.SystemName;

    private RegionEndpoint _Region {
        get { return RegionEndpoint.GetBySystemName(Region); }
    }

    private CognitoAWSCredentials _credentials;

    private CognitoAWSCredentials Credentials {
        get {
            if(_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _Region);
            return _credentials;
        }
    }

    private CognitoSyncManager _syncManager;

    private CognitoSyncManager SyncManager {
        get {
            if(_syncManager == null) {
                _syncManager = new CognitoSyncManager(Credentials, new AmazonCognitoSyncConfig { RegionEndpoint = _Region });
            }
            return _syncManager;
        }
    }

    public void Awake() {
        UnityInitializer.AttachToGameObject(this.gameObject);
        if (!FB.IsInitialized) {
            Debug.Log("THIS HAS BEEN TRIGGERED!");
            //Initialize the facebook SDK
            FB.Init(InitCallBack, OnHideUnity);
        }
        //AmazonDynamoDBClient client = new AmazonDynamoDBClient();
    }

    public void Start() {
    }

    //public void FacebookLoginCallback(IResult result) {
    //    if (FB.IsLoggedIn) {
    //        AddFacebookTokenToCognito();
    //    } else {
    //        Debug.Log("FB Login Error");
    //    }
    //}

    public void AddFacebookTokenToCognito(string accessToken) {
        Credentials.AddLogin("graph.facebook.com", accessToken);
    }

    private void InitCallBack() {
        if (FB.IsInitialized) {
            //Signal an app activation App Event
            FB.ActivateApp();
            Debug.Log("FB IS ACTIVATED!");
            //Continue with facebook SDK.
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        } else {
            Debug.Log("Failed to Initialize the Facebook SDK.");
        }
    }

    private void AuthCallback(ILoginResult result) {
        if (FB.IsLoggedIn) {
            Debug.Log("AUTH CALL BACK HAS BEEN TRIGGERED!");
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            AddFacebookTokenToCognito(aToken.Permissions.FirstOrDefault());
            Debug.Log(aToken.UserId);
            foreach (var perm in aToken.Permissions) {
                Debug.Log(perm);
            }
        } else {
            Debug.Log("User cancelled login");
        }
    }

    private void OnHideUnity(bool isGameShown) {
        if (!isGameShown)
        {
            //Pause the game - we will need to hide.
            Time.timeScale = 0;
        } else {
          //Resume the game - we're getting focus again.
            Time.timeScale = 1;
        }
    }
}
