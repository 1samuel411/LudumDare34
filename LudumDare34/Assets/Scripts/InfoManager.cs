using UnityEngine;
using System.Collections;
using System;

public class InfoManager : MonoBehaviour
{

    public string bought;
    public int coins;
    public int totalKills;
    public int totalScore;

    void Start()
    {

    }

    void Update()
    {

    }

    public static bool NewPlayer()
    {
        bool returnInfo = false;

        if (PlayerPrefs.HasKey("newUser18"))
            returnInfo = false;
        else
        {
            PlayerPrefs.SetString("newUser18", "true");
            InfoManager.SetInfo("bought", "");
            InfoManager.SetInfo("timeNeededHrs", "0");
            InfoManager.SetInfo("timeUsed", "00:00");
            InfoManager.SetInfo("coins", "0");
            InfoManager.SetInfo("kills", "0");
            InfoManager.SetInfo("score", "0");
            returnInfo = true;
        }
        return returnInfo;
    }

    public static string GetInfo(string name)
    {
        string returnInfo = "";
        returnInfo = PlayerPrefs.GetString(name);
        return returnInfo;
    }

    public static void SetInfo(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
    }

    public static void AddInfo(string name, int value)
    {
        int original = Int32.Parse(InfoManager.GetInfo(name));
        PlayerPrefs.SetString(name, (original + value).ToString());
    }
}
