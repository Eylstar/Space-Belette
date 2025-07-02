using UnityEngine;

[CreateAssetMenu(menuName = "ShipBuilderComponents")]
public class ShipBuilderComponents : ScriptableObject
{
    public GameObject[] HullPrefabs;
    public GameObject[] ComponentPrefabs;
    public Sprite[] ComponentIcons;

    public GameObject GetHullByName(string name)
    {
        foreach(var hull in HullPrefabs)
        {
            if (hull.name == name)
                return hull;
        }

        return null;
    }

    public GameObject GetComponentByName(string name)
    {
        foreach (var component in ComponentPrefabs)
        {
            if (component.name == name)
                return component;
        }

        return null;
    }
}