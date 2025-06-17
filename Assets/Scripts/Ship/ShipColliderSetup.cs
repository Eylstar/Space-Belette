using UnityEngine;
public class ShipColliderSetup : MonoBehaviour
{
    public BoxCollider[] boxColliders;
    public MeshCollider[] meshColliders;

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
            foreach (var boxCollider in boxColliders)
            {
                boxCollider.enabled = true;
            }
            foreach (var meshCollider in meshColliders)
            {
                meshCollider.enabled = false;
            }
        }
        else
        {
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
