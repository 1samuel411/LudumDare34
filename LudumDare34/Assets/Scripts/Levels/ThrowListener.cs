using UnityEngine;
using System.Collections;

public class ThrowListener : MonoBehaviour
{

    public BossEnemy boss;

    public void Throw()
    {
        if (!boss)
            boss = GetComponentInParent<BossEnemy>();

        if (boss)
            boss.ThrowAxe();
    }
}
