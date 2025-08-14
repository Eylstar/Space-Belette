using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
    public Action OnDestroyAction { get; set; }
    GameObject GetGameObject();
}