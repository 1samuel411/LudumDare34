using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class Level {
    public int Id;
    public string levelName;
    public int levelToLoad;
    public Sprite image;
    public bool locked;
}

