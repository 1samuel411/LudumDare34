using UnityEngine;
using System.Collections;

public class VoiceManager : MonoBehaviour
{

    public AudioClip[] startSounds;
    public AudioClip[] killSounds;
    public AudioClip[] dieSounds;
    public AudioClip[] damagedSounds;
    public AudioClip[] winSounds;
    public static VoiceManager instance;
    public AudioSource audioSource;
    private bool died;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if(LevelManager.instance.player)
            died = LevelManager.instance.player.baseHealth._died;
    }

    public void PlayStartSound()
    {
        if(!died)
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(startSounds[Random.Range(0, startSounds.Length )]);
    }

    public void PlayKillSound()
    {
        if(!died)
            if(!audioSource.isPlaying)
                audioSource.PlayOneShot(killSounds[Random.Range(0, killSounds.Length )]);
    }

    public void PlayDieSound()
    {
        died = true;
        audioSource.Stop();

        audioSource.PlayOneShot(dieSounds[Random.Range(0, dieSounds.Length )]);
    }

    public void PlayDamagedSound()
    {
        if (!died)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(damagedSounds[Random.Range(0, damagedSounds.Length)]);
        }
    }

    public void PlayWinSound()
    {
        if (!died)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(winSounds[Random.Range(0, winSounds.Length)]);
        }
    }
}
