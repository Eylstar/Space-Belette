using AYellowpaper.SerializedCollections;
using UnityEngine;
[CreateAssetMenu(fileName = "EngineList", menuName = "ScriptableObjects/EngineListSO")]
public class EngineListSO : ScriptableObject
{
    [SerializedDictionary("ID","Prefab")]
    public SerializedDictionary<int, GameObject> EngineList = new SerializedDictionary<int, GameObject>();
}
