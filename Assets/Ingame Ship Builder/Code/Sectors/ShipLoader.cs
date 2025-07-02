using UnityEngine;

public class ShipLoader : MonoBehaviour
{
    public ShipBuilderComponents Components;

    private void Awake()
    {
        SerializableShipData data = SerializableShipData.LoadFromFile(ShipBuilderController.SAVE_FOLDER+"TestSnapshot.ship");

        // Spawn the hull
        GameObject hull = Instantiate(Components.GetHullByName(data.HullName));

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

        hull.GetComponent<Ship>().isPlayerShip = true;
        Camera.main.GetComponent<CameraController>().SetTargetShip(hull.GetComponent<Ship>());
    }

}
