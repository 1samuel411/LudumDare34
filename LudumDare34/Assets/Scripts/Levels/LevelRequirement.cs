using UnityEngine;
using UnityEngine.Events;
using System.Collections;
//GameManager.instance.playerDetails.UnlockedLevels
public class LevelRequirement : MonoBehaviour
{

    public int levelNeeded;

    public UnityEvent levelMet;
    public UnityEvent levelNotMet;

    void Start()
    {
        if (GameManager.instance.playerDetails.UnlockedLevels < levelNeeded)
            levelNotMet.Invoke();
        else
            levelMet.Invoke();
    }

    void Update()
    {

    }
}
