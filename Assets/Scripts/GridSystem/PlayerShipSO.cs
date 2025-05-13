using System;
using System.Collections.Generic;
using UnityEngine;
using static Bloc;

[CreateAssetMenu(fileName = "PlayerShipSO", menuName = "ScriptableObjects/PlayerShipSO")]
public class PlayerShipSO : ScriptableObject
{
    public ShipData shipData;
}

[System.Serializable]
public class ShipData
{
    public string name;
    public List<ChildData> children = new List<ChildData>();
}

[System.Serializable]
public class ChildData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    [NonSerialized] public List<ChildData> children = new List<ChildData>();
    public bool isActive;
    public BlocType type;
    public int ID;
}

