using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseWeapon : BaseItem
{
    public bool isGunActive;
    public bool isAutomatic;
    public bool CanShoot;
    public GameObject muzzleFlash;
    public Vector2 pickupPosition;
    public float force;
    public AudioClip shootSound;
    public float weaponTriggerSpeed;   //Used for the wait Time between Shots.
    public WeaponEffects weaponEffect;
    public ProjectileType weaponType;
    public GameObject projectile;
    public GameObject bulletSpawnBox;
    protected IList<Projectile> _projectiles;
    private Projectile _projectile;
    public WeaponAttributes weaponAttribute;

    private new Animation animation;
    private new AudioSource audio;

    public void Awake()
    {
        animation = GetComponent<Animation>();
        audio = GetComponent<AudioSource>();
        if (bulletSpawnBox == null)
            bulletSpawnBox =  this.gameObject;

        this.name = (string.IsNullOrEmpty(this.name)) ? gameObject.name : this.name;
        Initialize();
    }

    protected virtual void Initialize() { }

    public int DamagePerBullet() {
        return _projectile.damage;
    }

    public void Update()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    protected virtual void SpawnBullets(float rotation = 0.0f) { // Make bullet
        bulletSpawnBox.transform.localEulerAngles = new Vector3(0, 0, rotation);
        GameObject newProjectileObject = GameObject.Instantiate(projectile, bulletSpawnBox.transform.position, (Quaternion)bulletSpawnBox.transform.rotation) as GameObject;
        newProjectileObject.transform.localScale = new Vector3((PlayerController.instance.direction == 1) ? 1 : -1, 1);
        _projectile = newProjectileObject.GetComponent<Projectile>(); //may need to fix this?
    }

    public IEnumerator FireWeapon() {
        while (isGunActive) {
            //Used to decrement ammo in weaponAttributes.
            if(!weaponAttribute.coreWeapon && weaponAttribute.usingAmmo)
                weaponAttribute.currentAmmo -= 1;

            SpawnBullets();
            // Make Flash
            GameObject newMuzzleflashObject = GameObject.Instantiate(muzzleFlash, bulletSpawnBox.transform.position, (Quaternion)bulletSpawnBox.transform.rotation) as GameObject;
            if(weaponTriggerSpeed < 0.25f) {
                newMuzzleflashObject.transform.parent = bulletSpawnBox.transform;
                newMuzzleflashObject.transform.localScale = new Vector3(1, 1, 1);
            } else {
                newMuzzleflashObject.transform.localScale = new Vector3((PlayerController.instance.direction == 1) ? 1 : -1, 1);
            }
            // Play animation
            if(animation)
                animation.Play();
            // Screenshake
            CameraManager.ShakeScreen(force, 2);
            // sound
            if(shootSound)
                audio.PlayOneShot(shootSound);

            yield return new WaitForSeconds(weaponTriggerSpeed);
        }
    }

    public void ActivateGun(bool activateGun) {
        isGunActive = activateGun;
        gameObject.SetActive(activateGun);
        if (activateGun) {
            ResetWeaponAttributes();
            StartCoroutine(FireWeapon());
            if (weaponAttribute.usingTimer) {
                weaponAttribute.checkTimer = true;
            }
        }
    }

    public void DestroyWeapon()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void ResetWeaponAttributes()
    {
        if (!weaponAttribute.coreWeapon) {
            weaponAttribute.currentMaxAmmo = weaponAttribute.maxAmountForAmmo;
            weaponAttribute.CurMaxAlottedTime = weaponAttribute.maxAmountForTime;
        }
    }

    public void DropWeapon()
    {
        //Awesome Drop Animation.
    }
}

public enum WeaponEffects {
    Normal = 0,
    Fire = 1,
    Ice = 2,
}