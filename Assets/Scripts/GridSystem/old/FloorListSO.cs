using AYellowpaper.SerializedCollections;
using UnityEngine;
[CreateAssetMenu(fileName = "FloorList", menuName = "ScriptableObjects/FloorListSO")]
public class FloorListSO : ScriptableObject
{
    [SerializedDictionary("ID","Prefab")]
    public SerializedDictionary<int, GameObject> FloorList = new SerializedDictionary<int, GameObject>();
}
