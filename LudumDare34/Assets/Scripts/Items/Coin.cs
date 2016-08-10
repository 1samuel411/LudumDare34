using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{

    public SpriteRenderer imageRenderer;
    public float despawnTimer;

    public AudioClip sound;

    public void OnEnable()
    {
        despawnTimer = Time.time + 6;
        imageRenderer.enabled = true;
        onOff = false;
        imageRenderer.enabled = true;
        gotten = false;
    }

    void Start()
    {
        InvokeRepeating("ToggleOnOff", 0, 0.25f);
        despawnTimer = Time.time + 6;
    }

    void Update()
    {
        CheckTimer();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player" && !gotten)
        {
            LevelManager.instance.coins++;
            
            Despawn();
        }
    }

    private new AudioSource audio;
    private bool gotten;
    private float gottenTimer;
    public void Despawn()
    {
        if (!audio)
            audio = GetComponentInParent<AudioSource>();

        gotten = true;
        audio.PlayOneShot(sound);

        imageRenderer.enabled = false;
        gottenTimer = Time.time + 1;
    }

    public void CheckTimer()
    {
        if (Time.time >= despawnTimer)
        {
            transform.parent.gameObject.SetActive(false);
        }
        if ((despawnTimer - Time.time) <= 3 && !onOff)
        {
            onOff = true;
        }
        if(Time.time >= gottenTimer && gotten)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
    private bool onOff;

    public void ToggleOnOff()
    {
        if(onOff && !gotten)
            imageRenderer.enabled = !imageRenderer.enabled;
    }
}
