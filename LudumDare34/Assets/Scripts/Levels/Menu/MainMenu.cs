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
    public Button watchAdsButton;
    public Button adsButton;

    public Text coinsText;
    public Text shopCoinsText;
    public Text scoreDisplayText;
    public Text killsDisplayText;

    public Text timeLeftCoinsText;

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


        // Ad Timer
        if (coinTimerChecked)
        {
            currentTimeMins = System.DateTime.UtcNow.Minute;
            currentTimeHrs = System.DateTime.UtcNow.Hour;

            DateTime timeNeededDateTime = new DateTime();
            timeNeededDateTime = Convert.ToDateTime(timeNeededHrs + ":00");

            TimeSpan timeSpan = new TimeSpan();
            timeSpan = CalculateTimeDifference(currentTimeHrs, timeUsedHrs, currentTimeMins, timeUsedMins);

            DateTime timeSpanDateTime = new DateTime();
            Debug.Log(timeSpan);
            if(timeSpan.Hours >= 0)
                timeSpanDateTime = Convert.ToDateTime(timeSpan.Hours + ":" + timeSpan.Minutes);

            TimeSpan displayTimeSpan = timeNeededDateTime - timeSpanDateTime;

            timeLeftCoinsText.text = "Hours: " + (displayTimeSpan.Hours) + "  Minutes: " + (displayTimeSpan.Minutes);

            if (timeNeededHrs <= 0 || (displayTimeSpan.Hours < 0 || displayTimeSpan.Minutes < 0 || (displayTimeSpan.Hours == 0 && displayTimeSpan.Minutes == 0)))
            {
                timeLeftCoinsText.text = "Ready!";
                watchAdsButton.interactable = true;
            }
            else
                watchAdsButton.interactable = false;
        }
    }

    int CalculateHourDifference(int aHr, int bHr, int aMin, int bMin)
    {
        double returnData = 0;
        DateTime aTime = new DateTime();
        aTime = Convert.ToDateTime(aHr + ":" + aMin);
        DateTime bTime = new DateTime();
        bTime = Convert.ToDateTime(bHr + ":" + bMin);
        returnData = (aTime - bTime).Hours;
        return (int)returnData;
    }

    TimeSpan CalculateTimeDifference(int aHr, int bHr, int aMin, int bMin)
    {
        TimeSpan returnData = new TimeSpan();
        DateTime aTime = new DateTime();
        aTime = Convert.ToDateTime(aHr + ":" + aMin);
        DateTime bTime = new DateTime();
        bTime = Convert.ToDateTime(bHr + ":" + bMin);
        returnData = (aTime - bTime);
        return returnData;
    }

    int CalculateMinuteDifference(int aHr, int bHr, int aMin, int bMin)
    {
        double returnData = 0;
        DateTime aTime = new DateTime();
        aTime = Convert.ToDateTime(aHr + ":" + aMin);
        DateTime bTime = new DateTime();
        bTime = Convert.ToDateTime(bHr + ":" + bMin);
        returnData = (aTime - bTime).Minutes;
        return (int)returnData;
    }

    public void PlayPress()
    {
        StartCoroutine(LoadLevel());
    }

    public void WatchAd()
    {
        AdManager.instance.ShowRewardedAd(RewardCallback);
    }
    private bool coinTimerChecked;

    public int timeUsedMins, timeUsedHrs, timeNeededHrs;
    public int currentTimeMins, currentTimeHrs;
    public void CheckCoinTimer()
    {
        coinTimerChecked = true;
        int minsLeft = System.DateTime.UtcNow.Minute;
        int hrsLeft = System.DateTime.UtcNow.Hour;
        int secsLeft = System.DateTime.UtcNow.Second;
        currentTimeMins = minsLeft;
        currentTimeHrs = hrsLeft;

        timeNeededHrs = Int32.Parse(InfoManager.GetInfo("timeNeededHrs"));
        timeUsedHrs = Int32.Parse(InfoManager.GetInfo("timeUsedHrs"));
        timeUsedMins = Int32.Parse(InfoManager.GetInfo("timeUsedMins"));
    }

    public void RewardCallback(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");

                coins += 20;
                timeNeededHrs = 4;
                timeUsedHrs = currentTimeHrs;
                timeUsedMins = currentTimeMins;
                InfoManager.SetInfo("timeUsedMins", currentTimeMins.ToString());
                InfoManager.SetInfo("timeUsedHrs", currentTimeHrs.ToString());
                InfoManager.SetInfo("timeNeededHrs", timeNeededHrs.ToString());
                InfoManager.SetInfo("coins", coins.ToString());

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
