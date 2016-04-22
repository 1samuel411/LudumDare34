using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour
{

    //public Canvas 
    public Button playButton;
    public Button adsButton;

    public Text coinsText;
    public Text shopCoinsText;
    public Text scoreDisplayText;
    public Text killsDisplayText;

    public static MainMenu instance;

    public int coins;
    public int totalKills;
    public int totalScore;

    public void Awake()
    {
        instance = this;
        if (!InfoManager.NewPlayer())
        {
            coins = Int32.Parse(InfoManager.GetInfo("coins"));
            totalKills = Int32.Parse(InfoManager.GetInfo("kills"));
            totalScore = Int32.Parse(InfoManager.GetInfo("score"));
        }
    }

    public void Update()
    {
        scoreDisplayText.text = totalScore.ToString();
        killsDisplayText.text = totalKills.ToString();
        coinsText.text = coins.ToString();
        shopCoinsText.text = coins.ToString();

        if (Advertisement.isSupported && Advertisement.isInitialized)
            adsButton.gameObject.SetActive(true);
        else
            adsButton.gameObject.SetActive(false);
    }

    public void PlayPress()
    {
        StartCoroutine(LoadLevel());
    }

    public void WatchAd()
    {
        AdManager.instance.ShowRewardedAd(RewardCallback);
    }

    public void RewardCallback(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");

                coins += 20;
                InfoManager.SetInfo("coins", coins.ToString());

                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                Popup.Create("Failed!", "The ad failed to be shown! \n \n Please contact us!", "Okay", "", true);
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    IEnumerator LoadLevel()
    {
        InfoManager.SetInfo("coins", coins.ToString());
        InfoManager.SetInfo("kills", totalKills.ToString());
        InfoManager.SetInfo("score", totalScore.ToString());
        CameraManager.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }
}
