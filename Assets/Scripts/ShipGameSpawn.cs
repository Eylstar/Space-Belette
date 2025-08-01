using UnityEngine;

public class ShipGameSpawn : MonoBehaviour
{
    [SerializeField] Transform spawnTransform;
    public ShipBuilderComponents Components;

    private void Awake()
    {
        SerializableShipData data = SerializableShipData.LoadFromFile(ShipBuilderController.SAVE_FOLDER + "TestSnapshot.ship");

        // Spawn the hull
        GameObject hull = Instantiate(Components.GetHullByName(data.HullName), spawnTransform.position, spawnTransform.rotation, spawnTransform);
        //hull.transform.localScale = Vector3.one*1.5f;

        // Spawn the components
        foreach (var mountedComponent in data.Components)
        {
            if (mountedComponent.ComponentName == "")
                continue;

            Instantiate(
                Components.GetComponentByName(mountedComponent.ComponentName.Replace("(Clone)", "").Trim()),
                mountedComponent.Position,
                Quaternion.Euler(mountedComponent.Rotation),
                hull.transform);
        }

        var shipcomp = hull.GetComponent<Ship>();
        shipcomp.isPlayerShip = true;
        shipcomp.ShipCost = data.ShipCost;
        //Camera.main.GetComponent<CameraController>().SetTargetShip(hull.GetComponent<Ship>());
    }

}
