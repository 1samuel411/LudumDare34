using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public enum ImpactType { thunder, explosion };
    public ImpactType impactType;
    public GameObject[] blood_effects;
    public float impactEffectSize;
    public float impactScreenshake;
    public int damage, originalDamage;
    public float knockbackForce;
    public float knockbackTime;
    public float projectileSpeed;
    public float bulletLife;
    public int impactAmounts = 1;

    private float currentBulletLife;

    private int direction;

    private float updatedTime;

    private TrailRenderer rendererTrail;

    public void Start() {
    }

    public void Update()
    {
        float speed = ((direction == 1) ? -projectileSpeed : projectileSpeed);
        this.transform.position += new Vector3(speed * Time.deltaTime, ((PlayerController.instance.direction == 1) ? 1 : 1) * Mathf.Sin(transform.rotation.z) * Time.timeScale);

        updatedTime += Time.deltaTime;
        if (updatedTime > currentBulletLife)
            this.gameObject.SetActive(false);
    }

    public void OnEnable() {
        if(!rendererTrail)
        {
            rendererTrail = GetComponent<TrailRenderer>();
        }
        rendererTrail.Clear();
        updatedTime = Time.time;
        currentBulletLife = Time.time + bulletLife;
        direction = PlayerController.instance.direction;
        transform.localScale = new Vector3((direction == 1) ? 1 : -1, 1, 1);
    }

    public void OnDisable() { }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            impactAmounts--;
            if (LevelManager.instance._wave == LevelManager.instance.maxLevel)
                impactAmounts = 0;
            BaseHealth enemyHealth = collider.GetComponent<BaseHealth>();
            if (enemyHealth._died == false)
            {
                BaseEntity enemyEntity = collider.GetComponent<BaseEntity>();
                enemyEntity.Knockback(knockbackForce, knockbackTime, direction);
                enemyHealth.DealDamage(damage);

                SpawnImpact();

                if (enemyEntity.baseHealth.currentHealth <= 0 && enemyEntity.baseHealth.type != BaseHealth.Type.skull && enemyEntity.baseHealth.type != BaseHealth.Type.cloud && enemyEntity.baseHealth.type != BaseHealth.Type.bat)
                    SpawnBlood();
            }
        }

        if(impactAmounts <= 0)
            this.gameObject.SetActive(false);
    }

    void SpawnBlood()
    {
        int randomBlood = Random.Range(7, 9);
        GameObject impactBloodEffect = LevelManager.instance.poolManager.SpawnAt(LevelManager.instance.spawnEffects[randomBlood], transform).gameObject;
        impactBloodEffect.transform.localScale = new Vector3(impactEffectSize, impactEffectSize, impactEffectSize);
        impactBloodEffect.transform.position = transform.position;
    }

    void SpawnImpact()
    {
        int randomBlood = Random.Range(3, 6);
        GameObject impactBloodEffect = LevelManager.instance.poolManager.SpawnAt(LevelManager.instance.spawnEffects[randomBlood], transform).gameObject;
        impactBloodEffect.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        impactBloodEffect.transform.localScale = new Vector3(impactEffectSize, impactEffectSize, impactEffectSize);
        impactBloodEffect.transform.position = transform.position;

        int explosion = (impactType == ImpactType.explosion) ? 2 : 0;
        GameObject impactEffect = LevelManager.instance.poolManager.SpawnAt(LevelManager.instance.spawnEffects[explosion], transform).gameObject;
        impactEffect.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        impactEffect.transform.localScale = new Vector3(impactEffectSize, impactEffectSize, impactEffectSize);
        impactEffect.transform.position = transform.position;

        Animation impactEffectAnimation = impactEffect.GetComponent<Animation>();

        CameraManager.ShakeScreen(impactScreenshake, 3);
    }
}

public enum ProjectileType
{
    ShortBullet = 0,
    LongBullet = 1,
    Beam = 2,
    ScatteredProjectile = 3
}