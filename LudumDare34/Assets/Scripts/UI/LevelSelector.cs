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

    void Start() {
        RefreshLevels(GameManager.instance.playerDetails.UnlockedLevels);
    }

    public static void AddLevelUnlocked() {
        GameManager.instance.playerDetails.UnlockedLevels++;
    }

    public void Open() {
        levels = levels.OrderBy(l => l.Id).ToArray();
        for(int i = 0; i < group.childCount; i++)
        {
            GameObject.Destroy(group.GetChild(i).gameObject);
        }
        for(int i = 0; i < levels.Length; i++) {
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

    public void RefreshLevels(int high) {
        foreach (var level in levels)
            level.locked = (level.Id > high);
    }
}
