using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class Options : MonoBehaviour
{

    public bool voiceEnabled = true;
    public bool musicEnabled = true;
    public bool soundEffectsEnabled = true;

    public AudioMixer voiceListener;
    public AudioMixer musicListener;
    public AudioMixer soundEffectsListener;

    public Image voiceImage;
    public Image musicImage;
    public Image soundImage;

    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    public float defaultVolumeVoice;
    public float defaultVolumeMusic;
    public float defaultVolumeEffects;

    public bool menuEnabled;

    public UnityEvent exitEvent;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("defaultVolumeVoice"))
        {
            voiceListener.GetFloat("volume", out defaultVolumeVoice);
            PlayerPrefs.SetFloat("defaultVolumeVoice", defaultVolumeVoice);
        }

        if (!PlayerPrefs.HasKey("defaultVolumeMusic"))
        {
            musicListener.GetFloat("volume", out defaultVolumeMusic);
            PlayerPrefs.SetFloat("defaultVolumeMusic", defaultVolumeMusic);
        }

        if (!PlayerPrefs.HasKey("defaultVolumeEffects"))
        {
            soundEffectsListener.GetFloat("volume", out defaultVolumeEffects);
            PlayerPrefs.SetFloat("defaultVolumeEffects", defaultVolumeEffects);
        }

        if (PlayerPrefs.HasKey("voiceEnabled"))
        {
            voiceEnabled = (PlayerPrefs.GetInt("voiceEnabled") == 1) ? true : false;
        }

        if (PlayerPrefs.HasKey("musicEnabled"))
        {
            musicEnabled = (PlayerPrefs.GetInt("musicEnabled") == 1) ? true : false;
        }

        if (PlayerPrefs.HasKey("effectsEnabled"))
        {
            soundEffectsEnabled = (PlayerPrefs.GetInt("effectsEnabled") == 1) ? true : false;
        }
    }

    public void ToggleOptions()
    {
        menuEnabled = !menuEnabled;
        Debug.Log("HI");
    }

    public void ToggleVoice()
    {
        voiceEnabled = !voiceEnabled;
        PlayerPrefs.SetInt("voiceEnabled", (voiceEnabled == true) ? 1 : 0);
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        PlayerPrefs.SetInt("musicEnabled", (musicEnabled == true) ? 1 : 0);
    }

    public void ToggleSoundEffects()
    {
        soundEffectsEnabled = !soundEffectsEnabled;
        PlayerPrefs.SetInt("effectsEnabled", (soundEffectsEnabled == true) ? 1 : 0);
    }

    void Update()
    {
        voiceImage.sprite = (voiceEnabled == true) ? musicOnSprite : musicOffSprite;
        musicImage.sprite = (musicEnabled == true) ? musicOnSprite : musicOffSprite;
        soundImage.sprite = (soundEffectsEnabled == true) ? musicOnSprite : musicOffSprite;

        voiceListener.SetFloat("volume", (voiceEnabled) ? defaultVolumeVoice : -80);
        musicListener.SetFloat("volume", (musicEnabled) ? defaultVolumeVoice : -80);
        soundEffectsListener.SetFloat("volume", (soundEffectsEnabled) ? defaultVolumeVoice : -80);

        Time.timeScale = (menuEnabled) ? 0 : 1;
    }

    public void ReturnToMenu()
    {
        Popup.Create("Return to Menu", "Do you really want to quit? You will lose all your progress!" + "\n \n" + "Tommy needs you!", "Yes", "No", false, PopupCallback);
    }

    public void PopupCallback(Popup.ResponseTypes response)
    {
        if (response == Popup.ResponseTypes.Accepted)
        {
            exitEvent.Invoke();
            LevelManager.instance.LoadLevel("mainMenu", false, true);
            Debug.Log("Accepted, Load mainMenu");
        }

        if (response == Popup.ResponseTypes.Declined)
        {
            Debug.Log("Declined, do nothing");
        }

        if (response == Popup.ResponseTypes.Exited)
        {
            exitEvent.Invoke();
        }

        if (response == Popup.ResponseTypes.None)
        {
            exitEvent.Invoke();
        }
    }
}
