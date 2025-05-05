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
        Bloc floorComponent = prefab.GetComponent<Bloc>();
        if (floorComponent != null && floorComponent.Icon != null)
        {
            icon.sprite = floorComponent.Icon;
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
