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

    public void Pullback()
    {
        if (!boss)
            boss = GetComponentInParent<BossEnemy>();

        if (boss)
            boss.PullAxeBack();
    }

    public void Roar()
    {
        if (!boss)
            boss = GetComponentInParent<BossEnemy>();

        if (boss)
            boss.audio.PlayOneShot(boss.roarSound, 2);
    }

    public void ListenForAttack()
    {
        if (!boss)
            boss = GetComponentInParent<BossEnemy>();

        if (boss)
            boss.axeLogic.listenForInfo = true;
    }

    public void StopListenForAttack()
    {
        if (!boss)
            boss = GetComponentInParent<BossEnemy>();

        if (boss)
            boss.axeLogic.listenForInfo = false;
    }
}
