using UnityEngine;
public class ShipColliderSetup : MonoBehaviour
{
    private BoxCollider[] boxColliders;
    private MeshCollider[] meshColliders;

    void Start()
    {
        boxColliders = GetComponentsInChildren<BoxCollider>();
        meshColliders = GetComponentsInChildren<MeshCollider>();
    }

    public void SetCollidersActive(bool isActive)
    {
        foreach (var boxCollider in boxColliders)
        {
            boxCollider.enabled = isActive;
        }
        foreach (var meshCollider in meshColliders)
        {
            meshCollider.enabled = isActive;
        }
    }
}
