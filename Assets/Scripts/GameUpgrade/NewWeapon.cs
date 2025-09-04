using System.Collections;
using UnityEngine;

public abstract class NewWeapon :  ScriptableObject, IWeaponLogic
{
    public abstract void Apply(int lvl, MonoBehaviour runner);
    public abstract IEnumerator WeaponEffect(int lvl);
}
