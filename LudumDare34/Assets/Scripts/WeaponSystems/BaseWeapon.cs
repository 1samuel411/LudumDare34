using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SVGImporter;

public class BaseWeapon : BaseItem
{

    public bool wepEnabled = true;

    public SVGImage weapon_image;
    public SVGImage weapon_background;

    public SVGAsset weapon_asset;
    public float scale;

    public enum HoldType { Onehanded, Twohanded };
    public HoldType holdType;
    public bool isGunActive;
    public bool isAutomatic;
    public bool CanShoot;
    public GameObject muzzleFlash;
    public Vector2 pickupPosition;
    public float pickupScale;
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
    public Weapons weapon;

    public float despawnTimer;
    private SVGRenderer imageRenderer;

    private new Animation animation;
    private new AudioSource audio;

    private bool _shooting;

    protected PoolManager _poolManager;
    protected int _spawnHandlerKey;
    protected SpawnObject _spawnObject;
    protected SpawnObject _muzzleEffect;
    private bool _isInitialized = false;

    public void Start()
    {
        weapon_background = LevelManager.instance.wepImages[0];
        if (weapon == Weapons.BeamWeapon)
            weapon_image = LevelManager.instance.wepImages[1];
        if (weapon == Weapons.M16)
            weapon_image = LevelManager.instance.wepImages[2];
        if (weapon == Weapons.Shotgun)
            weapon_image = LevelManager.instance.wepImages[3];
        if (weapon == Weapons.ThunderGun)
            weapon_image = LevelManager.instance.wepImages[4];
        if (weapon == Weapons.Uzi)
            weapon_image = LevelManager.instance.wepImages[5];

        imageRenderer = GetComponentInChildren<SVGRenderer>();

        animation = GetComponent<Animation>();
        audio = GetComponent<AudioSource>();
        if (bulletSpawnBox == null)
            bulletSpawnBox = this.gameObject;

        this.name = (string.IsNullOrEmpty(this.name)) ? gameObject.name : this.name;

    }

    protected virtual void Initialize()
    {
        if (!_isInitialized)
        {
            _poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<PoolManager>();
            SpawnHandlerDetails sph = new SpawnHandlerDetails()
            {
                initialSpawnAmount = 4,
                overflowMaxSpawnAmount = 8,
                setPoolManagerParent = false
            };
            _spawnHandlerKey = _poolManager.CreateNewSpawnHandler(bulletSpawnBox, sph);
            _spawnObject = _poolManager.AddToSpawnPool(projectile, true);
            _muzzleEffect = _poolManager.AddToSpawnPool(muzzleFlash, false);
            _projectile = _spawnObject.gameObject.GetComponent<Projectile>();
            _projectile.originalDamage = _projectile.damage;
            
            InvokeRepeating("ToggleOnOff", 0, 0.25f);
            _isInitialized = true;
        }
    }

    public virtual void OnEnable()
    {
        despawnTimer = Time.time + 6.7f;
        onOff = false;
        if(imageRenderer)
            imageRenderer.enabled = true;

        setVariables = false;
    }

    public int DamagePerBullet()
    {
        return _projectile.damage;
    }

    private bool setVariables;
    public void Update()
    {
        wepEnabled = LevelManager.instance.wepsEnabled;
        transform.localScale = new Vector3(1, 1, 1);
        Initialize();
        CheckTimer();

        // ShopVariables
        if (isGunActive && !setVariables)
        {
            setVariables = true;
            if (weapon == Weapons.Pistol)
            {
                _projectile.damage = _projectile.originalDamage + LevelManager.instance.pistolDamageAddition;
            }
        }
    }

    public void CheckTimer()
    {
        if (!isGunActive && (despawnTimer - Time.time) <= 3 && !onOff)
        {
            onOff = true;
        }
        if(Time.time >= despawnTimer && !isGunActive)
        {
            gameObject.SetActive(false);
        }
    }
    private bool onOff;

    public void ToggleOnOff()
    {
        if(!isGunActive && onOff)
        {
            imageRenderer.enabled = !imageRenderer.enabled;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            LevelManager.instance.player.EquipNextWeapon(this);
        }
    }

    protected virtual void SpawnBullets(float rotation = 4f)
    { // Make bullet
        bulletSpawnBox.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-rotation, rotation));
        _poolManager.SpawnAt(_spawnObject, bulletSpawnBox.transform);
        _spawnObject.gameObject.transform.localScale = new Vector3((PlayerController.instance.direction == 1) ? 1 : -1, 1);
    }

    public IEnumerator FireWeapon()
    {
        if (!_isInitialized || !_poolManager)
            Initialize();

        if (!_shooting)
        {
            _shooting = true;
            yield return new WaitForSeconds(0.1f);

            while (isGunActive)
            {
                if (wepEnabled)
                {
                    if (PlayerController.instance.baseHealth._died)
                        yield break;

                    //Used to decrement ammo in weaponAttributes.
                    if (!weaponAttribute.coreWeapon && weaponAttribute.usingAmmo)
                        weaponAttribute.currentAmmo -= 1;

                    SpawnBullets();
                    // Make Flash
                    GameObject muzzleEffect = null;
                    if (_poolManager)
                        muzzleEffect = _poolManager.SpawnAt(_muzzleEffect, bulletSpawnBox.transform).gameObject;
                    if (weaponTriggerSpeed < 0.25f)
                    {
                        muzzleEffect.GetComponent<AutoDestruct>().parent = bulletSpawnBox.transform;
                        muzzleEffect.transform.localScale = new Vector3((PlayerController.instance.direction == 1) ? -1 : 1, 1);
                    }
                    else
                    {
                        muzzleEffect.transform.localScale = new Vector3((PlayerController.instance.direction == 1) ? 1 : -1, 1);
                    }
                    // Play animation
                    if (animation)
                    {
                        animation.Play();
                    }
                    // Screenshake
                    CameraManager.ShakeScreen(force, 2);
                    // sound
                    if (shootSound && audio)
                        audio.PlayOneShot(shootSound);
                }
                yield return new WaitForSeconds(weaponTriggerSpeed);
            }
        }
    }

    public void ActivateGun(bool activateGun)
    {
        isGunActive = activateGun;
        gameObject.SetActive(activateGun);
        if (activateGun)
        {
            if(imageRenderer)
                imageRenderer.enabled = true;
            //ResetWeaponAttributes();
            StartCoroutine(FireWeapon());
            if (weaponAttribute.usingTimer)
            {
                weaponAttribute.checkTimer = true;
            }
        }
        else
        {
            _shooting = false;
        }
    }

    public void DestroyWeapon()
    {
        //GameObject.Destroy(this.gameObject);
        this.ActivateGun(false);
    }

    public void ResetWeaponAttributes()
    {
        if (!weaponAttribute.coreWeapon)
        {
            weaponAttribute.currentMaxAmmo = weaponAttribute.maxAmountForAmmo + LevelManager.instance.ammoAddition;
            weaponAttribute.CurMaxAlottedTime = weaponAttribute.maxAmountForTime + LevelManager.instance.timeAddition;
        }
    }

    public void DropWeapon()
    {
        //Awesome Drop Animation.
    }
}

public enum WeaponEffects
{
    Normal = 0,
    Fire = 1,
    Ice = 2,
}