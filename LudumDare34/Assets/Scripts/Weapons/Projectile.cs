using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    protected int _damage;
    public float _projectileSpeed;
    protected float _bulletLife;
    protected float _currentBulletLife;

    public int damage { get { return _damage; } }

    private int direction;

    public void Awake()
    {
        _projectileSpeed = 1;
        _currentBulletLife = Time.time + _bulletLife;
        direction = PlayerController.direction;
    }

    private float updatedTime;

    public void Update()
    {
        float speed = ((direction == 1) ? -_projectileSpeed : _projectileSpeed);
        this.transform.position += new Vector3( speed * Time.deltaTime, 0);

        updatedTime += Time.deltaTime;
        if (updatedTime > _currentBulletLife)
            GameObject.Destroy(this.gameObject);

    }

    /// <summary>
    /// Set the projectile Speed of the bullet.
    /// Only use this if the projectile has a speed.
    /// </summary>
    /// <param name="projectileSpeed">The Projectile Speed.</param>
    public void SetProjectileSpeed(float projectileSpeed) {
        _projectileSpeed = projectileSpeed;
    }
}

public enum ProjectileType {
    ShortBullet = 0,
    LongBullet = 1,
    Beam = 2,
    ScatteredProjectile = 3
}