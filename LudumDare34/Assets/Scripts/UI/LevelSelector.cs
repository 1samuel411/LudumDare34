using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelSelector : MonoBehaviour
{

    public Transform group;
    public Level[] levels;
    public int highestUnlockedLevel;

    [System.Serializable]
    public struct Level
    {
        public string levelName;
        public int levelToLoad;
        public Sprite image;
        public bool locked;
    }

    void Start() {
        RefreshLevels(InfoManager.GetInfo("unlockedLevels"));
    }

    public static void AddLevelUnlocked() {
        GameManager.instance.playerDetails.UnlockedLevels++;
        //InfoManager.SetInfo("unlockedLevels", InfoManager.GetInfo("unlockedLevels") + "," + levelIndex);
    }

    public void Open()
    {
        for(int i = 0; i < group.childCount; i++)
        {
            GameObject.Destroy(group.GetChild(i).gameObject);
        }
        for(int i = 0; i < levels.Length; i++)
        {
            GameObject levelObj = GameObject.Instantiate(Resources.Load("LevelItem")) as GameObject;
            LevelItem levelItem = levelObj.GetComponent<LevelItem>();
            levelItem.levelName = levels[i].levelName;
            levelItem.levelToLoad = levels[i].levelToLoad;
            levelItem.levelImage = levels[i].image;
            levelItem.levelLocked = levels[i].locked;
            levelObj.transform.SetParent(group);
            levelObj.transform.localScale = Vector3.one;
            levelObj.transform.position = Vector3.zero;
        }
    }

    public void RefreshLevels(string unlockedLevel) {
        int allLevels = levels.Count();
        for (int i = 0; i < allLevels; i++) {
            levels[i].locked = (i > highestUnlockedLevel);
        }
    }
}
