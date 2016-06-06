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
            GameManager.instance.playerDetails.CurrentTime = System.DateTime.UtcNow;

            DateTime timeNeededDateTime = new DateTime();
            timeNeededDateTime = Convert.ToDateTime(GameManager.instance.playerDetails.TimeNeededHours + ":00");

            TimeSpan timeSpan = new TimeSpan();
            timeSpan = CalculateTimeDifference(GameManager.instance.playerDetails.CurrentTime, timeUsed);
            DateTime timeSpanDateTime = new DateTime();
            if (timeSpan.Hours >= 0)
                timeSpanDateTime = Convert.ToDateTime(timeSpan.Hours + ":" + timeSpan.Minutes);

            TimeSpan displayTimeSpan = timeNeededDateTime - timeSpanDateTime;

            timeLeftCoinsText.text = "Hours: " + (displayTimeSpan.Hours) + "  Minutes: " + (displayTimeSpan.Minutes);

            if(GameManager.instance.playerDetails.TimeNeededHours <= 0 || (displayTimeSpan.Hours < 0 || displayTimeSpan.Minutes < 0 ||
                (displayTimeSpan.Hours == 0 && displayTimeSpan.Minutes == 0)) ||
                timeSpan.TotalHours > GameManager.instance.playerDetails.TimeNeededHours)
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
        GameManager.instance.playerDetails.CurrentTime = Convert.ToDateTime(currentServerTime);
    }

    public void RewardCallback(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                GameManager.instance.playerDetails.Coins += 20;
                GameManager.instance.playerDetails.TimeNeededHours = 4;
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

    public static IEnumerator LoadLevel(int level) {
        CameraManager.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(level);
        //SceneManager.LoadScene(2);
    }
}
