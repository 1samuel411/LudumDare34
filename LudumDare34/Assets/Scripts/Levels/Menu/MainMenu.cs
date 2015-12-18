using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    //public Canvas 
    public Button playButton;

	// Use this for initialization
	public void Start ()
	{
	    playButton = playButton.GetComponent<Button>();
	}

    public void PlayPress() {
        SceneManager.LoadScene(1);
    }
}
