﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour
{

    //public Canvas 
    public Button playButton;
    public Button watchAdsButton;
    public Button adsButton;

    public Text coinsText;
    public Text shopCoinsText;
    public Text iapShopCoinsText;
    public Text scoreDisplayText;
    public Text killsDisplayText;

    public Text timeLeftCoinsText;

    public static MainMenu instance;

    public int Coins {
        get { return GameManager.instance.playerDetails.Coins; }
        set { GameManager.instance.playerDetails.Coins = value; }
    }
    public int totalKills {
        get { return GameManager.instance.playerDetails.MaxKills; }
        set { GameManager.instance.playerDetails.MaxKills = value; }
    }
    public int totalScore {
        get { return GameManager.instance.playerDetails.HighScore; }
        set { GameManager.instance.playerDetails.HighScore = value; }
    }

    public int timeNeededHrs {
        get { return GameManager.instance.playerDetails.TimeNeededHours; }
        set { GameManager.instance.playerDetails.TimeNeededHours = value; }
    }

    public DateTime currentTime {
        get { return GameManager.instance.playerDetails.CurrentTime; }
        set { GameManager.instance.playerDetails.CurrentTime = value; }
    }

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        scoreDisplayText.text = totalScore.ToString();
        killsDisplayText.text = totalKills.ToString();
        coinsText.text = Coins.ToString();
        shopCoinsText.text = Coins.ToString();
        iapShopCoinsText.text = Coins.ToString();

        if (Advertisement.isSupported && Advertisement.isInitialized)
            adsButton.gameObject.SetActive(true);
        else
            adsButton.gameObject.SetActive(false);


        // Ad Timer
        if (coinTimerChecked)
        {
            currentTime = System.DateTime.UtcNow;

            DateTime timeNeededDateTime = new DateTime();
            timeNeededDateTime = Convert.ToDateTime(timeNeededHrs + ":00");

            TimeSpan timeSpan = new TimeSpan();
            timeSpan = CalculateTimeDifference(currentTime, timeUsed);
            DateTime timeSpanDateTime = new DateTime();
            if (timeSpan.Hours >= 0)
                timeSpanDateTime = Convert.ToDateTime(timeSpan.Hours + ":" + timeSpan.Minutes);

            TimeSpan displayTimeSpan = timeNeededDateTime - timeSpanDateTime;

            timeLeftCoinsText.text = "Hours: " + (displayTimeSpan.Hours) + "  Minutes: " + (displayTimeSpan.Minutes);

            if (timeNeededHrs <= 0 || (displayTimeSpan.Hours < 0 || displayTimeSpan.Minutes < 0 || (displayTimeSpan.Hours == 0 && displayTimeSpan.Minutes == 0)) || timeSpan.TotalHours > timeNeededHrs)
            {
                timeLeftCoinsText.text = "Ready!";
                watchAdsButton.interactable = true;
            }
            else
                watchAdsButton.interactable = false;
        }
    }

    TimeSpan CalculateTimeDifference(DateTime a, DateTime b)
    {
        TimeSpan returnData = new TimeSpan();
        returnData = (a - b);
        return returnData;
    }

    public void PlayPress()
    {
    }

    public void WatchAd()
    {
        AdManager.instance.ShowRewardedAd(RewardCallback);
    }
    private bool coinTimerChecked;

    public DateTime timeUsed;

    public void CheckCoinTimer()
    {
        coinTimerChecked = true;
        string currentServerTime = System.DateTime.UtcNow.ToString();
        currentTime = Convert.ToDateTime(currentServerTime);
    }

    public void RewardCallback(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                Coins += 20;
                timeNeededHrs = 4;
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                Popup.Create("Failed!", "You need to watch the full ad!", "Okay", "", true);
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                Popup.Create("Failed!", "The ad failed to be shown! \n \n Please contact us!", "Okay", "", true);
                break;
        }
    }

    public static IEnumerator LoadLevel(int level)
    {
        //MainMenu.instance.playerInformation.Put("coins", MainMenu.instance.Coins.ToString());
        //MainMenu.instance.playerInformation.Put("maxKills", MainMenu.instance.totalKills.ToString());
        //MainMenu.instance.playerInformation.Put("highScore", MainMenu.instance.totalScore.ToString());
        //InfoManager.SetInfo("coins", MainMenu.instance.coins.ToString());
        //InfoManager.SetInfo("kills", MainMenu.instance.totalKills.ToString());
        //InfoManager.SetInfo("score", MainMenu.instance.totalScore.ToString());
        CameraManager.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(2);
    }
}
