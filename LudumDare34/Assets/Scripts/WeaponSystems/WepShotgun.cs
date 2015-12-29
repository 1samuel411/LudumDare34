using UnityEngine;
using System.Collections;

public class WepShotgun : BaseWeapon
{
    public float spraySpread = 0;
    public int sprayAmount = 1;

    protected override void SpawnBullets(float rotation = 0.0f)
    {
        for (int i = 0; i < sprayAmount; i++)
        {
            rotation = 0;
            switch (i)
            {
                case 0:
                    rotation = -spraySpread;
                    break;
                case 1:
                    rotation = 0;
                    break;
                case 2:
                    rotation = spraySpread;
                    break;
            }
            base.SpawnBullets(rotation);
        }
    }
}
