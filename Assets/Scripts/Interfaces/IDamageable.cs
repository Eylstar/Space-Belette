using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
    public Action OnDestroy { get; set; }
    GameObject GetGameObject();
}