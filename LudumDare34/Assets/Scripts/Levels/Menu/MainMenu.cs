using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{

    //public Canvas 
    public Button playButton;

    public Text coinsText;
    public Text scoreDisplayText;
    public Text killsDisplayText;

    public static MainMenu instance;

    public int coins;
    public int totalKills;
    public int totalScore;

    public void Awake()
    {
        instance = this;
        playButton = playButton.GetComponent<Button>();
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
    }

    public void PlayPress()
    {
        StartCoroutine(LoadLevel());
    }

    public void WatchAd()
    {
        coins += 20;
        InfoManager.SetInfo("coins", coins.ToString());
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
