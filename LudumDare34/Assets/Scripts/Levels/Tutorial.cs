using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour
{

    public GameObject mainUI;

    public static Tutorial instance;

    public bool finishedTutorial;
    private bool finishedTutorialFinal;

    public TutorialStage[] stages;
    public int currentStage;

    public Sprite completedIcon;

    public string levelToLoad;

    private bool enemySpawned;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LevelManager.instance.wepsEnabled = false;
        LevelManager.instance.spawnEnemies = false;
        LevelManager.instance.moveOn = false;
    }

    void Update()
    {
        if (finishedTutorial && !finishedTutorialFinal)
        {
            finishedTutorialFinal = true;
            for(int i = 0; i < stages.Length; i++)
            {
                stages[i].uiObj.SetActive(false);
            }
            LevelManager.instance.spawnEnemies = true;
        }

        if(LevelManager.instance.enemiesKilled == 2 && !enemySpawned)
        {
            enemySpawned = true;
            StartCoroutine(FinishTutorial());
        }

        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].uiObj.SetActive(!finishedTutorial ? ((i == currentStage) ? true : false) : false);

            LevelManager.instance.spawnEnemies = (!finishedTutorial) ? stages[i].spawnEnemies : true;

            if (stages[i].spawnEnemy && !stages[i].spawnedEnemy && i == currentStage)
            {
                Debug.Log("Spawn Enemy!");
                stages[i].spawnedEnemy = true;
                LevelManager.instance.SpawnEnemyRegardless();
            }
        }

        if (stages[currentStage].completed)
        {
            if (stages[currentStage].nextIndex == -1)
            {
                finishedTutorial = true;
            }
            else
            {
                currentStage = stages[currentStage].nextIndex;
            }
        }
    }

    public IEnumerator FinishTutorial()
    {
        InfoManager.SetInfo("finishedTutorial", "1");
        if(GameManager.instance.playerDetails.UnlockedLevels < 1)
            GameManager.instance.playerDetails.UnlockedLevels = 1;
        Notification.Notify("Tutorial Master", "Congratualations you have completed the tutorial!", completedIcon);
        yield return new WaitForSeconds(3);
        Tutorial.instance = null;
        LevelManager.instance.LoadLevel(levelToLoad, false, true);
    }

    public void CompleteStage(string stageName)
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].name == stageName)
            {
                if (stages[i].spawnEnemy)
                {
                    stages[i].spawnedEnemy = true;
                    LevelManager.instance.SpawnEnemyRegardless();
                }
                stages[i].completed = true;
                break;
            }
        }
    }

    public void GoToStage(string stageName)
    {
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i].name == stageName)
            {
                currentStage = stages[i].index;
                break;
            }
        }
    }

    public void PreviousStage()
    {
        currentStage--;
    }

    [System.Serializable]
    public struct TutorialStage
    {
        public string name;
        public GameObject uiObj;
        public int index;
        public bool completed;
        public bool spawnEnemies;
        public bool spawnEnemy;
        [HideInInspector]
        public bool spawnedEnemy;
        public int nextIndex;
    } 
}
