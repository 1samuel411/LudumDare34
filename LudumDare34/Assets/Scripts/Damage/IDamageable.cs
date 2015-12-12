using UnityEngine;
using System.Collections;

public interface IDamageable : IDestroyable
{
    int DealDamage(int damage);
}
