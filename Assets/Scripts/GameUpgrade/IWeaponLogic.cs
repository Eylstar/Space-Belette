using System.Collections;
using UnityEngine;

interface IWeaponLogic 
{
    void Apply(int lvl, MonoBehaviour runner);
    IEnumerator WeaponEffect(int lvl);
}
