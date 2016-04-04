using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class MainMenu : MonoBehaviour {
    public Canvas MainCanvas;
    public Canvas SettingsCanvas;

    public void Awake() {
        SettingsCanvas.enabled = false;
    }

    public void SettingsOn() {
        SettingsCanvas.enabled = true;
        MainCanvas.enabled = false;
    }

    public void ReturnOn() {
        SettingsCanvas.enabled = false;
        MainCanvas.enabled = true;
    }

    public void LoadOn() {
        SceneManager.LoadScene(1);
    }
}
