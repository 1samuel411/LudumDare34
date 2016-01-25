using UnityEngine;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using SVGImporter;
using UnityStandardAssets.ImageEffects;

public class BaseHealth : MonoBehaviour, IDamageable
{

    public enum Type { player, bat, skull, spider, cloud };
    public Type type;
    
    public int currentHealth = 1;
    private int maxHealth;

    public float dissolveSpeed;
    public bool zoomable = true;
    public bool dissolveable = true;
    private bool dissolving;
    private float _dissolveTime;

    public bool healthChanged = true;
    public bool _died;

    private Material[] materials;
    private SVGRenderer[] renderers;

    private Animator animator;

    private SpawnObject sObj;
    private BaseEntity _base;
    private VignetteAndChromaticAberration _vignette;
    
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
        animator = GetComponentInChildren<Animator>();
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
        _base = this.gameObject.GetComponent<BaseEntity>();
        _vignette = Camera.main.GetComponent<VignetteAndChromaticAberration>();
    }

    bool diedEffectPlayed = false;

    void Update()
    {
        if(type == Type.player)
        {
            animator.SetBool("dead", _died);
            LevelManager.instance.healthImage.fillAmount = (float) currentHealth / (float)maxHealth;
            if (_died)
            {
                _vignette.intensity += 5 * Time.deltaTime;
                _vignette.intensity = Mathf.Clamp(_vignette.intensity, 0, 20);
                if (!diedEffectPlayed)
                {
                    diedEffectPlayed = true;
                    // Add effect
                    CameraManager.ShakeScreen(1.6f, 0.3f);
                    CameraManager.ZoomIn(8, 2.4f, 4, 0.6f, transform.position, 5, 100);
                }
            }
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

    #region IDamageable Members

    public int DealDamage(int damage)
    {
        if (type == Type.player)
        {
            if (!_base.isBoosting)
            {
                CurrentHealth -= damage;
            }

            if (!_died)
            {
                int randomNum = Random.Range(0, 100);
                if (randomNum > 60)
                    VoiceManager.instance.PlayDamagedSound();
            }
        }
        else
        {
            CurrentHealth -= damage;
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
                if (zoomInDecider > 75)
                {
                    // Add effect
                    CameraManager.ShakeScreen(2, 1.5f);
                    CameraManager.ZoomIn(8, 2.4f, 4, 0.3f, transform.position, 5, 1);
                }
            }

            // Add dissolve effect
            if (dissolveable)
                dissolving = true;

            if (type == Type.bat || type == Type.spider)
            {
                if(type == Type.skull)
                    LevelManager.instance.score += Random.Range(0, maxHealth + 2);
                else
                    LevelManager.instance.score += Random.Range(0, maxHealth * 5);
                //LevelManager.instance.totalEnmiesInWave--;
            }

            if(type == Type.player)
            {
                VoiceManager.instance.PlayDieSound();
            }

            if(type != Type.player)
            {
                int randomNum = Random.Range(0, 100);
                if(randomNum > 80)
                    VoiceManager.instance.PlayKillSound();
            }
        }
    }
    #endregion
}
