using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{

    public static AdManager instance;

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        if (Advertisement.isSupported)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Advertisement.Initialize("1060489");
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Advertisement.Initialize("1060490");
            }
        }
    }

    public void ShowRewardedAd(Action<ShowResult> callback = null)
    {
        Debug.Log("Showing ad...");
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = (callback == null) ? HandleShowResult : callback };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}