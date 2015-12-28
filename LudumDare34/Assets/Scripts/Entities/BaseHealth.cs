using UnityEngine;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using SVGImporter;

public class BaseHealth : MonoBehaviour, IDamageable
{

    public enum Type { player, bat, skull, spider };
    public Type type;

    public int currentHealth = 1;

    public float dissolveSpeed;
    public bool dissolveable = true;
    private bool dissolving;
    private float _dissolveTime;

    public bool _died;

    private Material[] materials;
    private SVGRenderer[] renderers;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (value <= 0)
                Die();
            else
                currentHealth = value;
        }
    }

    void Awake()
    {
        List<SVGRenderer> renderersList = new List<SVGRenderer>();
        List<Material> materialsList = new List<Material>();
        SpriteRenderer[] childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        SVGRenderer[] childrenMeshRenderers = GetComponentsInChildren<SVGRenderer>();
        for (int i = 0; i < childrenRenderers.Length; i++)
        {
            materialsList.Add(childrenRenderers[i].material);
        }

        for (int i = 0; i < childrenMeshRenderers.Length; i++)
        {
            renderersList.Add(childrenMeshRenderers[i]);
        }
        materials = materialsList.ToArray();
        renderers = renderersList.ToArray();
    }

    void Update()
    {
        if(dissolving)
        {
            _dissolveTime += dissolveSpeed * Time.deltaTime;
            for(int i = 0; i < renderers.Length; i ++)
            {
                //materials[i].SetFloat("_Amount", _dissolveTime);
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
        CurrentHealth -= damage;
        //Implement MissChance.
        return currentHealth;
    }

    public void DestroyThisObject()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void Die()
    {
        if (!_died)
        {
            _died = true;

            int zoomInDecider = Random.Range(0, 100);
            if (zoomInDecider > 75)
            {
                // Add effect
                CameraManager.ShakeScreen(2, 1.5f);
                CameraManager.ZoomIn(8, 2.4f, 4, 0.3f, transform.position, 5, 1);
            }

            // Add dissolve effect
            if (dissolveable)
                dissolving = true;

            gameObject.layer = 12;

            if(type == Type.bat || type == Type.spider)
            {
                LevelManager.instance.totalEnmiesInWave--;
            }
        }
    }
    #endregion
}
