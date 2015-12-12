using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    protected int _damage;
    protected float _projectileSpeed;
    protected float _bulletLife;

    public int damage { get { return _damage; } }

    public Projectile(int damage)
    {
        _damage = damage;
        _projectileSpeed = -1;
        _bulletLife = 3.0f;
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