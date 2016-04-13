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

    protected extWepPoolManager _poolManager;

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
        _poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<extWepPoolManager>();
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
            maxHealth = (defaultHealth * (LevelManager.instance._wave/2));
            maxHealth = Mathf.Clamp(maxHealth, 1, 8); 
        }
        currentHealth = maxHealth;
        _died = false;
    }

    #region IDamageable Members

    public int DealDamage(int damage)
    {
        if (type == Type.player)
            if (LevelManager.instance.player.isBoosting)
                return currentHealth;
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

    public virtual void SpawnDeath()
    {
        if (SpawnWeapon())
        {
            SpawnObject obj = _poolManager.RandomWeapon();
            LevelManager.instance.poolManager.SpawnAt(obj, this.transform);
        }

        if (SpawnCoin())
        {
            SpawnObject spawnedCoin = LevelManager.instance.poolManager.SpawnAt(LevelManager.instance.coinsSpawnObj, this.transform);
            spawnedCoin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-3, 3), Random.Range(3, 6)), ForceMode2D.Impulse);
        }
    }

    protected bool SpawnWeapon()
    {
        bool bOk = false;
        float num = Random.Range(0.0f, 1.0f);
        //Percent chance of spawning a weapon.
        if (num < 0.75f)
            bOk = true;
        return bOk;
    }

    protected bool SpawnCoin()
    {
        bool bOk = false;
        float num = Random.Range(0.0f, 1.0f);
        //Percent chance of spawning a weapon.
        if (num < 0.3f)
            bOk = true;
        return bOk;
    }

    public void Die()
    {
        if (!_died)
        {
            if (zoomable && type != Type.player)
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
                CameraManager.ShakeScreen(0.8f, 0.000001f);
                CameraManager.ZoomIn(8, 2.4f, 4, 1, transform.position, 5, 100);
                StartCoroutine(DeathSequence());
            }

            // Add dissolve effect
            if (dissolveable)
                dissolving = true;

            //gameObject.layer = 12;

            if (type == Type.bat || type == Type.spider || type == Type.bat)
            {
                LevelManager.instance.totalEnmiesInWave--;
                SpawnDeath();
            }

            if(type != Type.player && type != Type.cloud)
                LevelManager.instance.enemiesKilled++;

            if (addScore)
            {
                LevelManager.instance.score += (Random.Range(minScore, maxScore));
            }
            _died = true;
        }
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(0.5f);
        VoiceManager.instance.PlayDieSound();
        yield return new WaitForSeconds(3f);
        CameraManager.instance.FadeOut();
        yield return new WaitForSeconds(1.2f);
        LevelManager.instance.LoadLevel("gameover", true);
    }
    #endregion
}
