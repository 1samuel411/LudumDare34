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
    
    public Button googleButton;

    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    public float defaultVolumeVoice;
    public float defaultVolumeMusic;
    public float defaultVolumeEffects;

    public bool menuEnabled;

    public UnityEvent exitEvent;

    void Awake() {
        Initialize();

        voiceListener.GetFloat("volume", out defaultVolumeVoice);
        GameManager.instance.optionSettings.VoiceVolume = defaultVolumeVoice;
        musicListener.GetFloat("volume", out defaultVolumeMusic);
        GameManager.instance.optionSettings.MusicVolume = defaultVolumeMusic;
        soundEffectsListener.GetFloat("volume", out defaultVolumeEffects);
        GameManager.instance.optionSettings.EffectVolume = defaultVolumeEffects;

        //Are Volume Settings Enabled.
        voiceEnabled = GameManager.instance.optionSettings.VoiceEnabled;
        musicEnabled = GameManager.instance.optionSettings.MusicEnabled;
        soundEffectsEnabled = GameManager.instance.optionSettings.EffectEnabled;
    }

    private void Initialize() {
        googleButton.colors = GoogleColorBlock(googleButton.colors);
    }

    public void ToggleOptions()
    {
        menuEnabled = !menuEnabled;
    }

    public void ToggleVoice() {
        voiceEnabled = !voiceEnabled;
        GameManager.instance.optionSettings.VoiceEnabled = voiceEnabled;
    }

    public void ToggleMusic() {
        musicEnabled = !musicEnabled;
        GameManager.instance.optionSettings.MusicEnabled = musicEnabled;
    }

    public void ToggleSoundEffects() {
        soundEffectsEnabled = !soundEffectsEnabled;
        GameManager.instance.optionSettings.EffectEnabled = soundEffectsEnabled;
    }

    public void ToggleFacebookConnect() {
        
    }

    void Update()
    {
        voiceImage.sprite = (voiceEnabled) ? musicOnSprite : musicOffSprite;
        musicImage.sprite = (musicEnabled) ? musicOnSprite : musicOffSprite;
        soundImage.sprite = (soundEffectsEnabled) ? musicOnSprite : musicOffSprite;

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

    public void InvokeCognitoGoogle() {
		GameManager.instance.AuthenticateAndLoad(() => {
            googleButton.colors = GoogleColorBlock(googleButton.colors);
        });
    }

    private ColorBlock GoogleColorBlock(ColorBlock originalCb) {
        ColorBlock cb = originalCb;
        bool bOk = GameManager.instance.syncInitializer.syncManager.isGoogleLoggedIn;
#if UNITY_ANDROID || UNITY_IOS
        cb.normalColor = (bOk) ? Color.green : Color.red;
#else
        cb.normalColor = Color.red;
#endif
        cb.highlightedColor = cb.normalColor;
        cb.pressedColor = Color.blue;
        //Because the datasets have already been initialized,
        //they have to be re-sync'd to local.
        if (bOk) GameManager.instance.ResyncDatasets();
        return cb;
    }
}
