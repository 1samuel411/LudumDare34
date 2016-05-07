using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelItem : MonoBehaviour
{

    public Text nameText;
    public Image image;
    public GameObject locked;

    public string levelName;
    public int levelToLoad;
    public Sprite levelImage;
    public bool levelLocked = false;

    void Start()
    {

    }

    void Update()
    {
        locked.SetActive(levelLocked);
        nameText.text = levelName;
        image.sprite = levelImage;
    }

    public void LoadLevel()
    {
        StartCoroutine(MainMenu.LoadLevel(levelToLoad));
    }
}
