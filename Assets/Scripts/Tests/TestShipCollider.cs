using UnityEngine;

public class TestShipCollider : MonoBehaviour
{
    ShipColliderSetup shipColliderSetup;
    void Update()
    {
        while (shipColliderSetup == null)
        {
            shipColliderSetup = FindFirstObjectByType<ShipColliderSetup>();
            if (shipColliderSetup != null) break;
        }
    }
    public void Active(bool isActive)
    {
        if (shipColliderSetup == null) return;
        shipColliderSetup.SetCollidersActive(isActive);
    }
}
