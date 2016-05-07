using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookSync {

    private string message = string.Empty;
    public FacebookSync() {
        InitiateFacebook();
    }

    private void InitiateFacebook() {
        //Initialize the FaceBook authentication.
        if(!FB.IsInitialized) {
            FB.Init(this.OnInitComplete, this.OnHideUnity);
            //If user Facebook account is NOT logged in... then login.
            if(FB.IsLoggedIn)
                CallFBLogin();
        } else {
            InitiateFacebookLogin();
        }
    }

    private void InitiateFacebookLogin() {
        //If user Facebook account is logged in... then activate App.
        if(FB.IsLoggedIn)
            FB.ActivateApp();
        else
            CallFBLogin();
    }

    private void OnInitComplete() {
        message = "Success - Check log for details";
        message += "Success Response: OnInitComplete Called\n";
        message += string.Format(
            "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
            FB.IsLoggedIn,
            FB.IsInitialized);
        Debug.Log(message);

        if (FB.IsInitialized) {
            InitiateFacebookLogin();
        } else {
            Debug.Log("failed to Initialize the Facebook SDK.");
        }
    }

    private void OnHideUnity(bool isGameShown) {
        message = "Success - Check log for details";
        message += string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
        message += "Is game shown: " + isGameShown;
        Debug.Log(message);
    }

    //Logs in to facebook with Read Permissions.
    public void CallFBLogin() {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
        if(FB.IsLoggedIn)
            FB.ActivateApp();
    }

    //Read and publish calls split for authentication, and use purposes.
    private void CallFBLoginForPublish() {
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, AuthCallback);
    }

    private void AuthCallback(ILoginResult result) {
        if(FB.IsLoggedIn) {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log("The accessTokens User ID " + aToken.UserId);
            FB.ActivateApp();
        } else {
            Debug.Log("User cancelled login.");
        }
    }
}
