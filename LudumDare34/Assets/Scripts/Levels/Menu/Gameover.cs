using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{

    public Image image_waveBar;
    public Text text_wave;
    public Text text_score;
    public Text text_coins;
    public Text text_kills;

    public GameObject layout_highscores;
    public GameObject element_highscore;

    public float lerpSpeed;

    private float lerpedValueWave;
    private float lerpedValueScore;
    private float lerpedValueCoins;
    private float lerpedValueKills;

    private int displayValueWave;
    private int displayValueScore;
    private int displayValueCoins;
    private int displayValueKills;

    private GameInfo info;

    void Start()
    {
        info = GameObject.FindGameObjectWithTag("GameInfo").GetComponent<GameInfo>();
        GameManager.instance.playerDetails.Coins += info.coins;
        GameManager.instance.playerDetails.MaxKills += info.enemiesKilled;
        GameManager.instance.playerDetails.HighScore += info.score;

        if (GameManager.instance.playerDetails.UnlockedLevels < SceneManager.GetSceneByName(info.lastLevel).buildIndex)
        {
            GameManager.instance.playerDetails.UnlockedLevels = SceneManager.GetSceneByName(info.lastLevel).buildIndex;
        }

        Notification.Notify("Congrats", "Wave " + info.wave);

        InfoManager.SetInfo("coins", info.coins.ToString());
        InfoManager.AddInfo("kills", info.enemiesKilled);
        InfoManager.AddInfo("score", info.score);
    }

    void Update()
    {
        UpdateUI();

        LerpValues();

        UpdateValues();

        if(Input.GetMouseButtonDown(0))
        {
            UpdateValuesNow();
        }
    }

    void UpdateValuesNow()
    {
        displayValueCoins = info.coins;
        displayValueKills = info.enemiesKilled;
        displayValueScore = info.score;
        displayValueWave = info.wave;
    }

    void UpdateValues()
    {
        if (lerpedValueWave > displayValueWave)
            displayValueWave++;
        if (lerpedValueScore > displayValueScore)
            displayValueScore++;
        if (lerpedValueCoins > displayValueCoins)
            displayValueCoins++;
        if (lerpedValueKills > displayValueKills)
            displayValueKills++;

        displayValueWave = Mathf.Clamp(displayValueWave, 0, info.wave);
        displayValueScore = Mathf.Clamp(displayValueScore, 0, info.score);
        displayValueCoins = Mathf.Clamp(displayValueCoins, 0, info.coins);
        displayValueKills = Mathf.Clamp(displayValueKills, 0, info.enemiesKilled);
    }

    void LerpValues()
    {
        lerpedValueWave = Mathf.Lerp(lerpedValueWave, info.wave, lerpSpeed * Time.deltaTime);
        lerpedValueScore = Mathf.Lerp(lerpedValueScore, info.score, lerpSpeed * Time.deltaTime);
        lerpedValueCoins = Mathf.Lerp(lerpedValueCoins, info.coins, lerpSpeed * Time.deltaTime);
        lerpedValueKills = Mathf.Lerp(lerpedValueKills, info.enemiesKilled, lerpSpeed * Time.deltaTime);
    }

    void UpdateUI()
    {
        image_waveBar.fillAmount = lerpedValueWave / 25.0f;
        text_wave.text = displayValueWave.ToString();
        text_score.text = displayValueScore.ToString();
        text_coins.text = displayValueCoins.ToString();
        text_kills.text = displayValueKills.ToString();
    }

    public void Restart()
    {
        StartCoroutine(LoadLevel(info.lastLevel));
    }

    public void Menu()
    {
        StartCoroutine(LoadLevel("mainMenu"));
    }

    IEnumerator LoadLevel(string level)
    {
        Destroy(info.gameObject);
        CameraManager.instance.FadeOut();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(level);
    }
}
