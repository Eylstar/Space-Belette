using UnityEngine;
using System.Collections;

public class ShockWaveSO : NewWeapon
{
    private Coroutine weaponEffectCoroutine;
    float cooldown = 0.0f;

    // Variables

    public override void Apply(int lvl, MonoBehaviour runner)
    {
        if (weaponEffectCoroutine == null)
        {
            weaponEffectCoroutine = runner.StartCoroutine(WeaponEffect(lvl));
        }
    }

    public override IEnumerator WeaponEffect(int lvl)
    {
        //Weapon effect logic here

        yield return new WaitForSeconds(cooldown);
        weaponEffectCoroutine = null;
    }
}