using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TilePlacer;

public class ContentButton : MonoBehaviour
{
    TextMeshProUGUI text;
    GameObject _prefab;
    public void SetContent(string name, GameObject prefab)
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = name;
        _prefab = prefab;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        FindFirstObjectByType<TilePlacer>().floorPrefab = _prefab;
    }
}
