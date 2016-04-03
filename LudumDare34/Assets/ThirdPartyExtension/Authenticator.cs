using UnityEngine;
using System.Collections;
using Amazon.CognitoIdentity;
using System.Collections.Generic;
using ThirdParty.Json.LitJson;
using System;
using System.Threading;
using Amazon;
using Amazon.Runtime.Internal;


//This is the Unathorized/Guest  Authenticator.
public class Authenticator: CognitoAWSCredentials {

    const string PROVIDER_NAME = "BlazeWolf.com";
    const string IDENTITY_POOL = "us-east-1:50893fe8-8071-4143-893c-04974e42f0d9";
    static readonly RegionEndpoint REGION = RegionEndpoint.USEast1;

    private string login = null;

    public Authenticator(string loginAlias)
        : base(IDENTITY_POOL, REGION) {
        login = loginAlias;
    }

    protected override IdentityState RefreshIdentity() {
        IdentityState state = null;
        ManualResetEvent waitLock = new ManualResetEvent(false);
        MainThreadDispatcher.ExecuteCoroutineOnMainThread(ContactProvider((s) => {
            state = s;
            waitLock.Set();
        }));
        waitLock.WaitOne();
        return state;
    }

    IEnumerator ContactProvider(Action<IdentityState> callback) {
        WWW www = new WWW("http://example.com/?username=" + login);
        yield return www;
        string response = www.text;

        JsonData json = JsonMapper.ToObject(response);

        //The backend has to send us back an Identity and a OpenID token
        string identityId = json["IdentityId"].ToString();
        string token = json["Token"].ToString();

        IdentityState state = new IdentityState(identityId, PROVIDER_NAME, token, false);
        callback(state);

    }
}
