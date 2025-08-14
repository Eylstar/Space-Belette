using System;
using UnityEngine;

public class XPCollider : MonoBehaviour
{
    [SerializeField] private ShipGameplayManager shipGameplayManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<XPContainer>(out XPContainer xpContainer) && !xpContainer.triggered)
        {
            shipGameplayManager.GainExperience(xpContainer.experience);
            xpContainer.triggered = true;
            xpContainer.SetPlayer(shipGameplayManager.gameObject);
            xpContainer.StartMoving();
        }
    }
}
