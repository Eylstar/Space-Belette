using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TilePlacer;

public class ContentButton : MonoBehaviour
{
    TextMeshProUGUI text;
    GameObject _prefab;
    [SerializeField] Image icon;
    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetContent(string name, GameObject prefab)
    {
        text.text = name;
        _prefab = prefab;
        Floor floorComponent = prefab.GetComponent<Floor>();
        if (floorComponent != null && floorComponent.icon != null)
        {
            icon.sprite = floorComponent.icon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        FindFirstObjectByType<TilePlacer>().floorPrefab = _prefab;
    }
}
