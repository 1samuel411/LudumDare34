using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelSelector : MonoBehaviour
{

    public Transform group;
    public Level[] levels;
    public List<int> unlockedLevels = new List<int>();

    [System.Serializable]
    public struct Level
    {
        public string levelName;
        public int levelToLoad;
        public Sprite image;
        public bool locked;
    }

    void Start()
    {
        RefreshLevels(InfoManager.GetInfo("unlockedLevels"));
    }

    public static void AddLevelUnlocked(string levelIndex)
    {
        InfoManager.SetInfo("unlockedLevels", InfoManager.GetInfo("unlockedLevels") + "," + levelIndex);
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

    public void RefreshLevels(string unlockedLevel)
    {
        this.unlockedLevels.Clear();

        unlockedLevel = unlockedLevel.Replace(",", "");
        char[] unlockedLevelsChars = unlockedLevel.ToCharArray();
        for (int i = 0; i < unlockedLevelsChars.Length; i++)
        {
            this.unlockedLevels.Add(Int32.Parse(unlockedLevelsChars[i].ToString()));
        }
        for(int i = 0; i < this.levels.Length; i++)
        {
            this.levels[i].locked = true;
        }
        for(int i = 0; i < this.unlockedLevels.Count; i++)
        {
            levels[this.unlockedLevels[i]].locked = false;
        }
    }
}
