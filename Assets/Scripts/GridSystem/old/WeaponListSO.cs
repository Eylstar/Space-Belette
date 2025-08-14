using AYellowpaper.SerializedCollections;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponList", menuName = "ScriptableObjects/WeaponListSO")]
public class WeaponListSO : ScriptableObject
{
    [SerializedDictionary("ID","Prefab")]
    public SerializedDictionary<int, GameObject> WeaponList = new SerializedDictionary<int, GameObject>();
}
