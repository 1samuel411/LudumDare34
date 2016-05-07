using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{

    public SpriteRenderer imageRenderer;
    public float despawnTimer;

    public void OnEnable()
    {
        despawnTimer = Time.time + 6;
        imageRenderer.enabled = true;
        onOff = false;
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
        if(collider.tag == "Player")
        {
            LevelManager.instance.coins++;
            Despawn();
        }
    }

    public void Despawn()
    {
        transform.parent.gameObject.SetActive(false);
    }

    public void CheckTimer()
    {
        if (Time.time >= despawnTimer)
        {
            Despawn();
        }
        if ((despawnTimer - Time.time) <= 3 && !onOff)
        {
            onOff = true;
        }
    }
    private bool onOff;

    public void ToggleOnOff()
    {
        if(onOff)
            imageRenderer.enabled = !imageRenderer.enabled;
    }
}
