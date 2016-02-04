using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public class extWepPoolManager : PoolManager {

    public BaseWeapon[] WeaponObjects;
    public List<SpawnObject> weaponSpawnObjects;

    protected override void Initalize() {
        base.Initalize();
        weaponSpawnObjects = new List<SpawnObject>();
        int value = 0;
        foreach (BaseWeapon wep in WeaponObjects) {
            value = CreateNewSpawnHandler(string.Format("{0}SpawnHandler"
                                                , wep.weapon.ToString()), 3);
            SpawnObject obj = AddToSpawnPool(wep.gameObject, value);
            Spawn(obj).gameObject.SetActive(false);
            weaponSpawnObjects.Add(obj);
        }
    }

    public SpawnObject RandomWeapon()
    {
        SpawnObject obj;
        if(!weaponSpawnObjects.Any())
            throw new Exception("Weapon Objects does not contain weapons in Spawn Pool.");
        int num = Random.Range(0, weaponSpawnObjects.Count);
        SpawnObject weaponSpawnObject = weaponSpawnObjects.ElementAt(num);

        return weaponSpawnObject;
        //KeyValuePair<int, SpawnObject> spawnObj = allSPawnObjects.Where(o => o.Value.gameObject.activeSelf == false).
        //    FirstOrDefault(s => s.Key.Equals(weaponSpawnObject.spawnObjectKey));


        //obj = (!spawnObj.Equals(default(KeyValuePair<int, SpawnObject>)))
        //    ? spawnObj.Value : RandomWeapon();
        //return obj;
    }
}

public enum Weapons {
    Uzi = 0,
    M16 = 1,
    Shotgun = 2,
    ThunderGun = 3,
    BeamWeapon = 4,
    Pistol = 5
}