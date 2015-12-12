using UnityEngine;
using System.Collections;

public class Projectile: MonoBehaviour {
    public int damage;
    public float knockbackForce;
    public float knockbackTime;
    public float projectileSpeed;
    public float bulletLife;
    public float currentBulletLife;

    private int direction;

    private float updatedTime;

    public void Awake() {
        updatedTime = Time.time;
        currentBulletLife = Time.time + bulletLife;
        direction = PlayerController.instance.direction;
    }

    public void Update() {
        float speed = ((direction == 1) ? -projectileSpeed : projectileSpeed) + (PlayerController.instance.rigidbody.velocity.x / 1.5f);
        this.transform.position += new Vector3(speed * Time.deltaTime, 0);

        updatedTime += Time.deltaTime;
        if(updatedTime > currentBulletLife)
            RemoveBullet();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "Enemy") {
            BaseHealth enemyHealth = collider.GetComponent<BaseHealth>();
            if(enemyHealth._died == false) {
                BaseEntity enemyEntity = collider.GetComponent<BaseEntity>();
                enemyEntity.Knockback(knockbackForce, knockbackTime);
                enemyHealth.DealDamage(damage);
            }
        }
        RemoveBullet();
    }

    void RemoveBullet() {
        GameObject.Destroy(this.gameObject);
    }
}

public enum ProjectileType {
    ShortBullet = 0,
    LongBullet = 1,
    Beam = 2,
    ScatteredProjectile = 3
}