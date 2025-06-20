using System;
using UnityEngine;
public class ShipColliderSetup : MonoBehaviour
{
    public BoxCollider[] boxColliders;
    public MeshCollider[] meshColliders;
    public static event Action OnShipEnter;
    public static event Action OnShipExit;

    void OnEnable()
    {
        boxColliders = GetComponentsInChildren<BoxCollider>();
        meshColliders = GetComponentsInChildren<MeshCollider>();
        if (boxColliders.Length == 0 && meshColliders.Length == 0)
        {
            Debug.LogWarning("No colliders found in children of " + gameObject.name);
        }
        else
        {
            SetCollidersActive(true);
        }
    }

    public void SetCollidersActive(bool isActive)
    {
        Debug.LogWarning("Setting colliders");
        if (isActive == true)
        {
            OnShipExit?.Invoke();
            Debug.LogWarning("Enabling box colliders and disabling mesh colliders");
            foreach (var boxCollider in boxColliders)
            {
                boxCollider.enabled = true;
            }
            foreach (var meshCollider in meshColliders)
            {
                meshCollider.enabled = false;
            }
        }
        else if (isActive == false)
        {
            OnShipEnter?.Invoke();
            Debug.LogWarning("Disabling box colliders and enabling mesh colliders");
            foreach (var boxCollider in boxColliders)
            {
                boxCollider.enabled = false;
            }
            foreach (var meshCollider in meshColliders)
            {
                meshCollider.enabled = true;
            }
        }
    }
}
