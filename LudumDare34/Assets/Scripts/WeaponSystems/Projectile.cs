using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public GameObject impact_effect;
    public float impactEffectSize;
    public float impactScreenshake;
    public int damage;
    public float knockbackForce;
    public float knockbackTime;
    public float projectileSpeed;
    public float bulletLife;

    private float currentBulletLife;

    private int direction;

    private float updatedTime;

    public void Start()
    {
        updatedTime = Time.time;
        currentBulletLife = Time.time + bulletLife;
        direction = PlayerController.instance.direction;
    }

    public void Update()
    {
        float speed = ((direction == 1) ? -projectileSpeed : projectileSpeed);
        this.transform.position += new Vector3(speed * Time.deltaTime, 0);

        updatedTime += Time.deltaTime;
        if (updatedTime > currentBulletLife)
            RemoveBullet();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            BaseHealth enemyHealth = collider.GetComponent<BaseHealth>();
            if (enemyHealth._died == false)
            {
                BaseEntity enemyEntity = collider.GetComponent<BaseEntity>();
                enemyEntity.Knockback(knockbackForce, knockbackTime);
                enemyHealth.DealDamage(damage);

                SpawnImpact();
            }
        }
        RemoveBullet();
    }

    void SpawnImpact()
    {
        GameObject impactEffectObj = Instantiate(impact_effect, transform.position, Quaternion.identity) as GameObject;
        impactEffectObj.transform.localScale = new Vector3(impactEffectSize, impactEffectSize, impactEffectSize);
        Animation impactEffectAnimation = impactEffectObj.GetComponent<Animation>();
        impactEffectAnimation.Play();
        CameraManager.ShakeScreen(impactScreenshake, 3);
    }

    void RemoveBullet()
    {
        GameObject.Destroy(this.gameObject);
    }
}

public enum ProjectileType
{
    ShortBullet = 0,
    LongBullet = 1,
    Beam = 2,
    ScatteredProjectile = 3
}