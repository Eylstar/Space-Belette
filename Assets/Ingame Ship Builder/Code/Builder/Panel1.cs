using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Panel1 : MonoBehaviour
{
    public Dropdown HullSelector;
    public Dropdown FileSelector;

    private ShipBuilderController buildController;
    private Dictionary<string, string> savedShips;

    private void Awake()
    {
        buildController = GetComponentInParent<ShipBuilderController>();
        savedShips = new Dictionary<string, string>();

        // Populate file selector with existing filenames
        string[] shipSaves = Directory.GetFiles(ShipBuilderController.SAVE_FOLDER, "*.ship");
        foreach(string filePath in shipSaves)
        {
            savedShips.Add(Path.GetFileNameWithoutExtension(filePath), filePath);
        }
        FileSelector.AddOptions(new List<string>(savedShips.Keys));
    }

    void Start()
    {
        List<string> dropdownOptions = new List<string>();
        foreach (GameObject hullPrefab in buildController.ComponentList.HullPrefabs)
        {
            dropdownOptions.Add(hullPrefab.name);
        }
        HullSelector.AddOptions(dropdownOptions);
    }

    public void OnCreateNewClicked()
    {
        string hullName = HullSelector.options[HullSelector.value].text;

        if (buildController.Ship != null)
        {
            if (buildController.ShipFullCost > 0)
                buildController.PlayerStats.ChangeMoneyUp(buildController.ShipFullCost);
            buildController.ShipFullCost = 0;
            GameObject.Destroy(buildController.Ship.gameObject);
        }
        var hullPrefab = buildController.ComponentList.GetHullByName(hullName);
        var hullCost = 0;
        var hullComponent = hullPrefab.GetComponent<ConstructionHull>();
        if (hullComponent != null)
            hullCost = hullComponent.ShipCost;

        if (buildController.PlayerStats.Money < hullCost)
        {
            buildController.ShowError("Not enough currency to create this ship!");
            return;
        }
        buildController.PlayerStats.ChangeMoneyDown(hullCost);
        buildController.ShipFullCost = hullCost;
        buildController.Ship = Instantiate(buildController.ComponentList.GetHullByName(hullName)).GetComponent<ConstructionHull>();
        buildController.Ship.gameObject.name = hullName;
        Camera.main.GetComponent<OrbitCameraController>().SetTarget(buildController.Ship.transform);
    }

    public void OnLoadFromFileClicked()
    {
        string shipName = FileSelector.options[FileSelector.value].text;
        string fileName = savedShips[shipName];

        SerializableShipData data = SerializableShipData.LoadFromFile(fileName);

        if (data == null)
            return;

        if (buildController.Ship != null)
            GameObject.Destroy(buildController.Ship.gameObject);

        buildController.LoadShipFromData(data);
        Debug.Log("Ship loaded from file: " + fileName);
    }

    public void OnNextClicked()
    {
        buildController.OnNextClicked();
    }
}
