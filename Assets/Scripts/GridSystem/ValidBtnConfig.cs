using UnityEngine;
using UnityEngine.UI;

public class ValidBtnConfig : MonoBehaviour
{
    ShipConstruct shipConstruct;
    Button btn;
    private void Start()
    {
        shipConstruct = FindFirstObjectByType<ShipConstruct>();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(shipConstruct.ReassignChildrenToPlayerShip);
    }
}
