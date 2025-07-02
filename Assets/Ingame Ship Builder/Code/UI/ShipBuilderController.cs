using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShipBuilderController : MonoBehaviour
{
#if UNITY_EDITOR
    public static string SAVE_FOLDER = "Assets/Ingame Ship Builder/Ships/";
#else
    public static string SAVE_FOLDER = "Ships/";
#endif

    public ShipBuilderComponents ComponentList;

    [Tooltip("Panels, each for one step of the ship builder")]
    public GameObject[] Panels;
    public Image[] ProgressIndicators;

    [HideInInspector]
    public ConstructionHull Ship;
    private int currentStep = 0;

    private void Start()
    {
        OpenTab(0);

        // Load latest snapshot
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        SerializableShipData data = SerializableShipData.LoadFromFile(SAVE_FOLDER+"TestSnapshot.ship");
        if (data == null)
            return;

        LoadShipFromData(data);
    }

    public void OnNextClicked()
    {
        Panels[currentStep].SetActive(false);
        ProgressIndicators[currentStep].color = Color.grey;
        currentStep = Mathf.Clamp(currentStep + 1, 0, Panels.Length-1);
        Panels[currentStep].SetActive(true);
        ProgressIndicators[currentStep].color = Color.white;
    }

    public void OnPreviousClicked()
    {
        Panels[currentStep].SetActive(false);
        ProgressIndicators[currentStep].color = Color.grey;
        currentStep = Mathf.Clamp(currentStep - 1, 0, Panels.Length - 1);
        Panels[currentStep].SetActive(true);
        ProgressIndicators[currentStep].color = Color.white;
    }

    public void OpenTab(int index)
    {
        for(int i=0; i<Panels.Length; i++)
        {
            Panels[i].SetActive(i == index);
            ProgressIndicators[i].color = (i == index) ? Color.white : Color.gray;
        }
        currentStep = index;
    }

    public void LoadShipFromData(SerializableShipData data)
    {

        // Spawn the hull
        Ship = Instantiate(ComponentList.GetHullByName(data.HullName.Replace("(Clone)", "").Trim())).GetComponent<ConstructionHull>();
        Ship.name = Ship.name.Replace("(Clone)", "").Trim();

        for (int i = 0; i < data.Components.Count; i++)
        {
            if (data.Components[i].ComponentName == "")
                continue;

            // Find hardpoint on which a part will be mounted
            HullHardpoint hp = null;
            foreach (HullHardpoint hardpoint in Ship.MountedComponents.Keys)
            {
                if (hardpoint.transform.position == data.Components[i].Position)
                {
                    hp = hardpoint;
                    break;
                }
            }

            // Mount component
            Ship.MountComponent(hp, ComponentList.GetComponentByName(data.Components[i].ComponentName.Replace("(Clone)", "").Trim()));
        }

        Camera.main.GetComponent<OrbitCameraController>().SetTarget(Ship.transform);
    }
}
