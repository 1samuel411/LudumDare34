using UnityEngine;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using SVGImporter;

public class BaseHealth : MonoBehaviour, IDamageable
{

    public enum Type { player, bat, skull, spider, cloud };
    public Type type;
    
    public int currentHealth = 1;
    private int maxHealth;
    private int defaultHealth;

    public float dissolveSpeed;
    public bool zoomable = true;
    public bool dissolveable = true;
    private bool dissolving;
    private float _dissolveTime;

    public bool healthChanged = true;
    public bool _died;

    public bool addScore;
    private bool _addScore;
    public int minScore;
    public int maxScore;

    private Material[] materials;
    private SVGRenderer[] renderers;

    private Animator animator;

    private SpawnObject sObj;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (value <= 0)
                Die();

            currentHealth = value;
        }
    }

    void Awake()
    {
        _addScore = addScore;
        animator = GetComponentInChildren<Animator>();
        defaultHealth = currentHealth;
        maxHealth = currentHealth;
        List<SVGRenderer> renderersList = new List<SVGRenderer>();
        List<Material> materialsList = new List<Material>();
        SpriteRenderer[] childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        SVGRenderer[] childrenMeshRenderers = GetComponentsInChildren<SVGRenderer>();
        for (int i = 0; i < childrenRenderers.Length; i++) {
            materialsList.Add(childrenRenderers[i].material);
        }

        for (int i = 0; i < childrenMeshRenderers.Length; i++) {
            renderersList.Add(childrenMeshRenderers[i]);
        }
        materials = materialsList.ToArray();
        renderers = renderersList.ToArray();

        sObj = this.gameObject.GetComponent<SpawnObject>();
    }

    void Update()
    {
        if(type == Type.player)
        {
            animator.SetBool("dead", _died);
            LevelManager.instance.healthImage.fillAmount = (float) currentHealth / (float)maxHealth;
        }
        else if(dissolving)
        {
            _dissolveTime += dissolveSpeed * Time.deltaTime;
            for(int i = 0; i < renderers.Length; i ++) {
                Color newColor = renderers[i].color;
                newColor.a = newColor.a - _dissolveTime;
                renderers[i].color = newColor;
            }

            if(_dissolveTime > 1)
            {
                DestroyThisObject();
            }
        }
    }

    public void OnEnable()
    {
        addScore = _addScore;
        if(type != Type.player && type != Type.skull)
        {
            maxHealth = (defaultHealth * LevelManager.instance._wave); 
        }
        currentHealth = maxHealth;
        _died = false;
    }

    #region IDamageable Members

    public int DealDamage(int damage)
    {
        CurrentHealth -= damage;
        //Implement MissChance.
        if (type == Type.player)
        {
            // Add effect
            CameraManager.ShakeScreen(2, 1.5f);
            CameraManager.ZoomIn(8, 2.4f, 4, 0.3f, transform.position, 5, 0.5f);

            int zoomInDecider = Random.Range(0, 100);
            if (zoomInDecider > 60)
            {
                VoiceManager.instance.PlayDamagedSound();
            }
        }
        return currentHealth;
    }

    public void DestroyThisObject()
    {
        // Stop Dissolving
        dissolving = false;
        _died = false;

        // Reset Opacity
        for (int i = 0; i < renderers.Length; i++)
        {
            Color newColor = renderers[i].color;
            newColor.a = 1;
            renderers[i].color = newColor;
        }

        // Deactivate
        if(sObj != null)
            PoolManager.DeactivateObjects(sObj);
        //GameObject.Destroy(this.gameObject);
    }

    public void Die()
    {
        if (!_died)
        {
            _died = true;

            if (zoomable)
            {
                int zoomInDecider = Random.Range(0, 100);
                if (zoomInDecider > 80)
                {
                    // Add effect
                    CameraManager.ShakeScreen(2, 1.5f);
                    CameraManager.ZoomIn(8, 2.4f, 4, 0.3f, transform.position, 5, 0.5f);
                    VoiceManager.instance.PlayKillSound();
                }
            }

            if(type == Type.player)
            {
                // Add effect
                CameraManager.ShakeScreen(1.2f, 0.1f);
                CameraManager.ZoomIn(8, 2.4f, 4, 0.6f, transform.position, 5, 100);
                VoiceManager.instance.PlayDieSound();
            }

            // Add dissolve effect
            if (dissolveable)
                dissolving = true;

            //gameObject.layer = 12;

            if (type == Type.bat || type == Type.spider || type == Type.bat)
            {
                LevelManager.instance.totalEnmiesInWave--;
            }

            if(addScore)
            {
                LevelManager.instance.score += (Random.Range(minScore, maxScore));
            }
        }
    }
    #endregion
}
