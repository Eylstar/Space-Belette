using UnityEngine;
using UnityEngine.UI;

public class Panel2 : MonoBehaviour
{
    public Texture2D EraserCursorTexture;
    public VerticalLayoutGroup ComponentSelector;
    public GameObject IconPrefab;

    private ShipBuilderController buildController;

    private PanelState state;
    private GameObject selectedComponent;
    private static Vector3 eraserOffset = new Vector3(22, 13, 0);
    [SerializeField] int freeSlot = 0;

    private enum PanelState
    {
        SELECT_COMPONENT, // Select a component from the UI to add to model
        DELETE_COMPONENTS, // Click on a component on the model to remove
        MOUNT_COMPONENT // Click on a hardpoint to add the selected component to model
    }

    private void Awake()
    {
        buildController = GetComponentInParent<ShipBuilderController>();
        if (buildController.ComponentList.ComponentPrefabs.Length != buildController.ComponentList.ComponentIcons.Length)
            Debug.LogWarning("ShipBuilderController prefab and icon list should match!");
    }

    void Start()
    {
        for (int i = 0; i < buildController.ComponentList.ComponentPrefabs.Length; i++)
        {
            ShipProp prop = buildController.ComponentList.ComponentPrefabs[i].GetComponent<ShipProp>();

            var component = buildController.ComponentList.ComponentPrefabs[i];
            var sprite = buildController.ComponentList.ComponentIcons[i];
            GameObject componentImage = GameObject.Instantiate(IconPrefab, ComponentSelector.transform);

            componentImage.GetComponent<PropInfo>().SetPropInfo(prop.PropName, prop.Cost);
            componentImage.GetComponent<Image>().sprite = sprite;
            var componentIndex = i;
            componentImage.GetComponent<Button>().onClick.AddListener(() => OnComponentSelected(componentIndex));
        }
    }

    private void OnComponentSelected(int componentIndex)
    {
        var prefab = buildController.ComponentList.ComponentPrefabs[componentIndex];
        var prop = prefab.GetComponent<ShipProp>();
        int cost = prop != null ? prop.Cost : 0;

        if (!(prop != null && prop.IsStartProp && freeSlot == 0) && buildController.PlayerStats.Money < cost)
        {
            buildController.ShowError("Not enought currency to buy this");
            return;
        }

        state = PanelState.MOUNT_COMPONENT;
        Cursor.SetCursor(null, Vector3.zero, CursorMode.Auto);
        if (selectedComponent != null)
            Destroy(selectedComponent);

        selectedComponent = Instantiate(buildController.ComponentList.ComponentPrefabs[componentIndex]);
        selectedComponent.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                string tag = hit.transform.gameObject.tag;
                if (tag == "Hardpoint")
                {
                    if (state == PanelState.MOUNT_COMPONENT)
                    {
                        var prop = selectedComponent.GetComponent<ShipProp>();
                        int cost = prop != null ? prop.Cost : 0;
                        if (prop.IsStartProp && freeSlot == 0)
                        {
                            freeSlot++;
                        }
                        else
                        {
                            buildController.PlayerStats.ChangeMoneyDown(cost);
                            buildController.ShipFullCost += cost;
                        }
                        selectedComponent.layer = LayerMask.NameToLayer("Default");
                        buildController.Ship.MountComponent(hit.transform.gameObject.GetComponent<HullHardpoint>(), selectedComponent);
                        GameObject.Destroy(selectedComponent);
                        selectedComponent = null;
                    }
                }
                if (state == PanelState.DELETE_COMPONENTS)
                {
                    Debug.Log("Raycast hit object: " + hit.transform.name);
                    if (tag == "Hardpoint")
                    {
                        var hardpoint = hit.transform.gameObject.GetComponent<HullHardpoint>();
                        if (hardpoint != null && buildController.Ship.MountedComponents.TryGetValue(hardpoint, out GameObject mounted))
                        {
                            var prop = mounted.GetComponent<ShipProp>();
                            int refund = prop != null ? prop.Cost : 0;
                            if (prop.IsStartProp && freeSlot > 0)
                            {
                                freeSlot--;
                            }
                            else
                            {
                                buildController.PlayerStats.ChangeMoneyUp(refund);
                                buildController.ShipFullCost -= refund;
                            }
                            Debug.Log("Unmounting component: " + mounted.name + " from hardpoint: " + hardpoint.name);
                        }
                        buildController.Ship.UnmountComponent(hardpoint);
                    }
                    else
                    {
                        var prop = hit.transform.gameObject.GetComponent<ShipProp>();
                        int refund = prop != null ? prop.Cost : 0;
                        if (prop.IsStartProp && freeSlot > 0)
                        {
                            freeSlot--;
                        }
                        else
                        {
                            buildController.PlayerStats.ChangeMoneyUp(refund);
                            buildController.ShipFullCost -= refund;
                        }
                        buildController.Ship.UnmountComponent(hit.transform.gameObject);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == PanelState.MOUNT_COMPONENT)
            {
                state = PanelState.SELECT_COMPONENT;
                Destroy(selectedComponent);
                selectedComponent = null;
            }
            if (state == PanelState.DELETE_COMPONENTS)
            {
                state = PanelState.SELECT_COMPONENT;
                Cursor.SetCursor(null, Vector3.zero, CursorMode.Auto);
            }
        }
        if (selectedComponent != null)
        {
            var distanceToModel = Vector3.Distance(Camera.main.transform.position, buildController.Ship.transform.position);
            selectedComponent.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToModel));
        }
    }

    private void OnEnable()
    {
        state = PanelState.SELECT_COMPONENT;
    }


    public void OnRemoveClicked()
    {
        state = PanelState.DELETE_COMPONENTS;
        Destroy(selectedComponent);
        selectedComponent = null;
        Cursor.SetCursor(EraserCursorTexture, eraserOffset, CursorMode.Auto);
    }

    public void OnNextClicked()
    {
        buildController.OnNextClicked();
    }

    public void OnPreviousClicked()
    {
        if (buildController.Ship != null)
            GameObject.Destroy(buildController.Ship.gameObject);

        buildController.OnPreviousClicked();
    }
}
