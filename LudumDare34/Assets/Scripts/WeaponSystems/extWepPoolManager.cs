using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Random = UnityEngine.Random;

public class extWepPoolManager : PoolManager {

    public BaseWeapon[] WeaponObjects;
    public Dictionary<int, SpawnObject> weaponSpawnObjects;

    protected override void Initalize() {
        base.Initalize();
        weaponSpawnObjects = new Dictionary<int, SpawnObject>();
        int value = 0;
        foreach (BaseWeapon wep in WeaponObjects) {
            value = CreateNewSpawnHandler(string.Format("{0}SpawnHandler"
                                                , wep.weapon.ToString()), 3);
            SpawnObject obj = AddToSpawnPool(wep.gameObject, value);
            Spawn(obj);
            obj.DeactivateObject();
            weaponSpawnObjects.Add(value, obj);
        }
    }

    public SpawnObject RandomWeapon()
    {
        SpawnObject obj;
        if(!weaponSpawnObjects.Any())
            throw new Exception("Weapon Objects does not contain weapons in Spawn Pool.");
        IEnumerable<SpawnObject> allWeaponObjs = from allWeaps in weaponSpawnObjects
                                join allHandlers in allSpawnHandlers on allWeaps.Key 
                                equals allHandlers.Key //into weaponHandlers
                                select new SpawnObject();

        IEnumerable<SpawnObject> deactiveWeapons = allWeaponObjs.Where(s => s.gameObject.activeSelf.Equals(false));
        Debug.Log("The number of weapon Objects are: " + deactiveWeapons.Count());
        int num = Random.Range(0, deactiveWeapons.Count());
        obj = deactiveWeapons.ElementAt(num);

        return obj;
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