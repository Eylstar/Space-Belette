using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Panel3 : MonoBehaviour
{
    public InputField FilenameInput;

    private ShipBuilderController buildController;

    private void Awake()
    {
        buildController = GetComponentInParent<ShipBuilderController>();
    }

    public void OnTestFlightClicked()
    {
        // Remove ConstructionHull component
        GameObject ship = buildController.Ship.gameObject;
        SaveShipSnapshot();
        SceneManager.LoadScene("CharacterSelection");
    }

    public void SaveShipSnapshot()
    {
        SerializableShipData data = GetBuilderShipData();
        data.SaveToFile(ShipBuilderController.SAVE_FOLDER+"TestSnapshot");
    }

    public void SaveShipToFile()
    {
        SerializableShipData data = GetBuilderShipData();
        data.SaveToFile(ShipBuilderController.SAVE_FOLDER + FilenameInput.text);
        Debug.Log("Ship saved to file: " + FilenameInput.text);
    }

    private SerializableShipData GetBuilderShipData()
    {
        SerializableShipData data = new SerializableShipData();
        data.HullName = buildController.Ship.gameObject.name;
        data.ShipCost = buildController.ShipFullCost;
        data.Components = new System.Collections.Generic.List<SerializableComponentData>();
        foreach (var mountedComponent in buildController.Ship.MountedComponents)
        {
            SerializableComponentData component = new SerializableComponentData();
            component.Position = mountedComponent.Key.transform.position;
            component.Rotation = mountedComponent.Key.transform.eulerAngles;
            component.ComponentName = mountedComponent.Value == null ?
                "" : mountedComponent.Value.name.Replace("(Clone)", "").Trim();
            data.Components.Add(component);
        }

        return data;
    }

}
