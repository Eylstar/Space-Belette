using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Bloc;

public class ShipConstruct : MonoBehaviour
{
    public static ShipConstruct Instance { get; private set; }
    public GameObject PlayerShip;
    public GameObject gridObj;
    Pilot Pilot;
    FloorListSO floorListSO;
    EngineListSO engineListSO;
    WeaponListSO weaponListSO;
    PlayerShipSO playerShipSO;
    bool isCockpitPlaced = false;
    [SerializeField] int CockpitCount;
    bool isEnginePlaced = false;
    [SerializeField] int EngineCount;
    public int MaxWeight = 0;
    public int CurrentWeight = 0;
    [SerializeField] Slider weightSlider;
    [SerializeField] TextMeshProUGUI errorText;
    string errorLogs;
    [SerializeField] Button ValidBtn;

    private void Awake()
    {
        // Singleton classique
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // S'abonner à l'événement de chargement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
        PilotSelection.OnPilotSelected += OnPilotSelected;
        OnUtilPlaced.AddListener(UtilAdd);
        OnUtilRemoved.AddListener(UtilRemove);
    }

    private void OnDestroy()
    {
        // Désabonnement propre
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PilotSelection.OnPilotSelected -= OnPilotSelected;
        OnUtilPlaced.RemoveListener(UtilAdd);
        OnUtilRemoved.RemoveListener(UtilRemove);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Grid")
        {
            // Appelle ici la logique de OnEnable
            SetupShipConstruct();
        }
    }

    // Place ici tout le code de ton OnEnable (sauf le Singleton/DontDestroyOnLoad)
    private void SetupShipConstruct()
    {

        Debug.LogWarning("On setup le ShipConstruct");
        isCockpitPlaced = false;
        CockpitCount = 0;
        isEnginePlaced = false;
        EngineCount = 0;
        MaxWeight = 0;
        CurrentWeight = 0;
        ValidBtn = GameObject.FindGameObjectWithTag("ValidBtn").GetComponent<Button>();
        ValidBtn.onClick.AddListener(ReassignChildrenToPlayerShip);
        PlayerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        weightSlider = GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>();
        errorText = GameObject.FindGameObjectWithTag("LogError").GetComponent<TextMeshProUGUI>();
        gridObj = FindFirstObjectByType<GridManager>().gameObject;
        playerShipSO = Resources.Load<PlayerShipSO>("ScriptableObjects/PlayerShipSO");
        floorListSO = Resources.Load<FloorListSO>("ScriptableObjects/FloorList");
        engineListSO = Resources.Load<EngineListSO>("ScriptableObjects/EngineList");
        weaponListSO = Resources.Load<WeaponListSO>("ScriptableObjects/WeaponList");
        UpdateWeight();
    }

    private void UtilRemove(UtilityType type)
    {
        if (type == UtilityType.Cockpit)
        {
            CockpitCount--;
            if (CockpitCount != 1) isCockpitPlaced = false;
            if (CockpitCount == 1) isCockpitPlaced = true;
        }
        else if (type == UtilityType.Engine)
        {
            EngineCount--;
            if (EngineCount < 1) isEnginePlaced = false;
        }
    }

    void UtilAdd(UtilityType type)
    {
        if (type == UtilityType.Cockpit)
        {
            isCockpitPlaced = true;
            CockpitCount++;
            if (CockpitCount != 1) isCockpitPlaced = false;
        }
        else if (type == UtilityType.Engine)
        {
            isEnginePlaced = true;
            EngineCount++;
        }
    }
    private void OnPilotSelected(Pilot pilot)
    {
        Pilot = pilot;
        SceneLoader.LoadScene("MissionSelection");
    }

    private void Start()
    {
    }

    public void ReassignChildrenToPlayerShip()
    {
        errorLogs = string.Empty;
        if (!isCockpitPlaced || !isEnginePlaced || !ShipValidator.IsAllConnected(this, out errorLogs) || CurrentWeight > MaxWeight)
        {
            if (!isCockpitPlaced)
            {
                if (CockpitCount > 1)
                    errorLogs += $"You can have only one Cockpit !\n";
                else
                    errorLogs += $"No Cockpit !\n";
            }
            if (!isEnginePlaced) errorLogs += $"No Engine!\n";
            if (CurrentWeight > MaxWeight) errorLogs += $"Too heavy! Remove some blocs.\n";
            ShipUIConst.ShowError(errorText, errorLogs, this, 2f);
            Debug.Log($"Cockpit : {isCockpitPlaced} / Engine : {isEnginePlaced} / All Connected : {ShipValidator.IsAllConnected(this, out _)} / Weight : {CurrentWeight}/{MaxWeight}");
            return;
        }

        foreach (Transform tile in gridObj.transform)
        {
            Bloc bloc = tile.GetComponentInChildren<Bloc>();
            if (bloc != null)
            {
                DestroyInactiveWalls(bloc);
                bloc.transform.SetParent(PlayerShip.transform);
            }
        }
        SavePlayerShip();
    }

    private void DestroyInactiveWalls(Bloc bloc)
    {
        foreach (Transform menu in bloc.transform)
        {
            foreach (Transform sub in menu.transform)
            {
                if (sub.gameObject.name == "Room")
                {
                    foreach (Transform wall in sub.transform)
                    {
                        if (!wall.gameObject.activeInHierarchy)
                        {
                            Destroy(wall.gameObject);
                        }
                    }
                }
            }
        }
    }

    public void SavePlayerShip()
    {
        ShipData shipData = new ShipData { name = PlayerShip.name };
        ShipPersistence.SaveGameObjectData(PlayerShip, shipData.children);
        playerShipSO.shipData = shipData;
        Debug.Log("Données de PlayerShip sauvegardées dans le ScriptableObject");
        SceneManager.LoadScene("CharacterSelection");
    }

    public void LoadPlayerShip()
    {
        if (playerShipSO.shipData == null)
        {
            Debug.LogError("Aucune donnée de vaisseau trouvée dans le ScriptableObject");
            return;
        }
        ShipData shipData = playerShipSO.shipData;
        GameObject playerShip = new GameObject(shipData.name);
        ShipManager sm = playerShip.AddComponent<ShipManager>();
        sm.MainPilot = Pilot;
        ShipPersistence.LoadGameObjectData(playerShip, shipData.children, sm, floorListSO, engineListSO, weaponListSO);
        CenterPlayerShip(playerShip);
        PlayerShip = playerShip;
        playerShip.AddComponent<ShipColliderSetup>();
        sm.SpawnPilot();
        Debug.Log("PlayerShip reconstruit à partir du ScriptableObject");
    }

    private void CenterPlayerShip(GameObject playerShip)
    {
        Vector3 center = Vector3.zero;
        int childCount = 0;
        foreach (Transform child in playerShip.transform)
        {
            Vector3 tileCenter = child.localPosition + new Vector3(0.5f, 0, 0.5f);
            center += tileCenter;
            childCount++;
        }
        if (childCount > 0)
            center /= childCount;
        center += new Vector3(-3, 0, 2);
        foreach (Transform child in playerShip.transform)
        {
            child.localPosition -= center;
        }
    }

    internal void UpdateWeight()
    {
        weightSlider.maxValue = MaxWeight;
        weightSlider.value = CurrentWeight;
    }

    public void ShowError(float duration = 2f)
    {
        ShipUIConst.ShowError(errorText, errorLogs, this, duration);
    }

    private void HideError()
    {
        if (errorText != null)
            errorText.gameObject.SetActive(false);
    }
}
