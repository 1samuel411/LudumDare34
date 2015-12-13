using UnityEngine;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;

public class BaseHealth : MonoBehaviour, IDamageable
{

    public int currentHealth = 1;

    public float dissolveSpeed;
    public bool dissolveable = true;
    private bool dissolving;
    private float _dissolveTime;

    public bool _died;

    private Material[] materials;

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
        List<Material> materialsList = new List<Material>();
        SpriteRenderer[] childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < childrenRenderers.Length; i++)
        {
            materialsList.Add(childrenRenderers[i].material);
        }
        materials = materialsList.ToArray();
    }

    void Update()
    {
        if(dissolving)
        {
            _dissolveTime += dissolveSpeed * Time.deltaTime;
            for(int i = 0; i < materials.Length; i ++)
            {
                materials[i].SetFloat("_Amount", _dissolveTime);
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

            // Add effect
            CameraManager.ShakeScreen(2, 1.5f);
            CameraManager.ZoomIn(8, 2.4f, 4, 0.3f, transform.position, 5, 1);

            // Add dissolve effect
            if (dissolveable)
                dissolving = true;

            gameObject.layer = 12;
        }
    }
    #endregion
}
