using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class Panel1 : MonoBehaviour
{
    public Dropdown HullSelector;
    public Dropdown FileSelector;

    private ShipBuilderController buildController;
    private Dictionary<string, string> savedShips;
    PopUpSystem popUpSystem;

    private void Awake()
    {
        buildController = GetComponentInParent<ShipBuilderController>();
        savedShips = new Dictionary<string, string>();
        popUpSystem = FindFirstObjectByType<PopUpSystem>();
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

        EventTrigger trigger = HullSelector.gameObject.AddComponent<EventTrigger>();

        EventTrigger hullTrigger = HullSelector.gameObject.AddComponent<EventTrigger>();
        var hullEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        hullEntry.callback.AddListener((data) => ShowHullPopUpIfNotExpanded());
        hullTrigger.triggers.Add(hullEntry);
        var hullExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        hullExit.callback.AddListener((data) => HidePopUp());
        hullTrigger.triggers.Add(hullExit);

        EventTrigger fileTrigger = FileSelector.gameObject.AddComponent<EventTrigger>();
        var fileEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        fileEntry.callback.AddListener((data) => ShowFilePopUpIfNotExpanded());
        fileTrigger.triggers.Add(fileEntry);
        var fileExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        fileExit.callback.AddListener((data) => HidePopUp());
        fileTrigger.triggers.Add(fileExit);
    }

    private void ShowFilePopUpIfNotExpanded()
    {
        var dropdownList = GameObject.Find("Dropdown List");
        if (dropdownList == null)
        {
            ShowFilePopUp();
        }
    }

    private void ShowFilePopUp()
    {
        if (FileSelector.options == null || FileSelector.options.Count == 0)
            return;
        int index = FileSelector.value;
        if (index < 0 || index >= FileSelector.options.Count || index == 0)
            return;
        string fileName = FileSelector.options[index].text;
        string description = "";
        string price = "";
        if (savedShips.TryGetValue(fileName, out string filePath))
        {
            SerializableShipData data = SerializableShipData.LoadFromFile(filePath);
            if (data != null)
            {
                description = $"Base : {data.HullName}";
                description += $"\nTotal life : {data.ShipLife}";
                if (data.ShipRegen != 0)
                {
                    description += $"\nLife Regen : {data.ShipRegen}";
                }
                price = data.ShipCost.ToString("N0") + " $";
            }
        }
        popUpSystem.ShowPopUp(fileName, description, price);
    }
    private void ShowHullPopUpIfNotExpanded()
    {
        // Vérifie si le menu déroulant est déplié (présence d'un objet "Dropdown List" dans la hiérarchie)
        var dropdownList = GameObject.Find("Dropdown List");
        if (dropdownList == null)
        {
            ShowHullPopUp();
        }
    }

    private void ShowHullPopUp()
    {
        int index = HullSelector.value;
        var hullPrefab = buildController.ComponentList.HullPrefabs[index];
        string name = "Ship";
        string description = hullPrefab.name;
        int price = 0;
        int life = 0;
        int regen = 0;
        var hullComp = hullPrefab.GetComponent<ConstructionHull>();
        if (hullComp != null)
        {
            price = hullComp.ShipCost;
            life = hullComp.ShipLife;
            description += $"\nLife: {hullComp.ShipLife.ToString()}";
            if (hullComp.ShipRegen != 0)
            {
                regen = hullComp.ShipRegen;
                description += $"\nRegen: {hullComp.ShipRegen.ToString()}";
            }
        }

        popUpSystem.ShowPopUp(name, description, price.ToString("N0") + " $");
    }
    private void HidePopUp()
    {
        popUpSystem.HidePopUp();
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
        {
            buildController.PlayerStats.ChangeMoneyUp(buildController.ShipFullCost);
            GameObject.Destroy(buildController.Ship.gameObject);
        }

        buildController.LoadShipFromData(data);
        Debug.Log("Ship loaded from file: " + fileName);
    }

    public void OnNextClicked()
    {
        buildController.OnNextClicked();
    }
}
