using System.Collections.Generic;
using UnityEngine;

public static class ShipPersistence
{
    public static void SaveGameObjectData(GameObject gameObject, List<ChildData> childrenData)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;
            ChildData childData = new ChildData
            {
                name = child.name,
                position = child.localPosition,
                rotation = child.localRotation,
                scale = child.localScale,
                children = new List<ChildData>()
            };
            Bloc floorComponent = child.GetComponent<Bloc>();
            if (floorComponent != null)
            {
                childData.type = floorComponent.blocType;
                childData.ID = floorComponent.ID;
            }
            childrenData.Add(childData);
            SaveGameObjectData(child.gameObject, childData.children);
        }
    }

    public static void LoadGameObjectData(GameObject parent, List<ChildData> childrenData, ShipManager manager,
        FloorListSO floorListSO, EngineListSO engineListSO, WeaponListSO weaponListSO)
    {
        foreach (ChildData childData in childrenData)
        {
            GameObject child = null;
            switch (childData.type)
            {
                case Bloc.BlocType.Floor:
                    if (floorListSO.FloorList.TryGetValue(childData.ID, out GameObject floorPrefab))
                    {
                        child = Object.Instantiate(floorPrefab, parent.transform);
                        SetShipManager(child, manager);
                    }
                    break;
                case Bloc.BlocType.Utility:
                    if (engineListSO.EngineList.TryGetValue(childData.ID, out GameObject enginePrefab))
                    {
                        child = Object.Instantiate(enginePrefab, parent.transform);
                        SetShipManager(child, manager);
                        if (child.GetComponent<Bloc>().utilityType == Bloc.UtilityType.Cockpit)
                        {
                            manager.shipSpawners.Add(ShipManager.ShipSpawner.Cockpit, child.GetComponent<Bloc>().PlayerSpawn);
                        }
                    }
                    break;
                case Bloc.BlocType.Weapon:
                    if (weaponListSO.WeaponList.TryGetValue(childData.ID, out GameObject weaponPrefab))
                    {
                        child = Object.Instantiate(weaponPrefab, parent.transform);
                        SetShipManager(child, manager);
                    }
                    break;
            }
            if (child != null)
            {
                child.transform.localPosition = childData.position;
                child.transform.localRotation = childData.rotation;
                child.transform.localScale = childData.scale;
                SetWalls(childData, child);
            }
        }
    }

    private static void SetShipManager(GameObject comp, ShipManager manager)
    {
        Bloc cb = comp.GetComponent<Bloc>();
        manager.MaxLife += cb.LifeBonus;
        manager.CurrentLife += cb.LifeBonus;
        manager.ShipBlocs.Add(cb);
        if (cb.blocType == Bloc.BlocType.Weapon || cb.utilityType == Bloc.UtilityType.Cockpit)
        {
            manager.ShootBlocs.Add(cb);
        }
    }

    private static void SetWalls(ChildData childData, GameObject child)
    {
        if (childData.type != Bloc.BlocType.Utility)
        {
            foreach (Transform menu in child.transform)
            {
                if (menu.gameObject.name == "Graph")
                {
                    foreach (Transform sub in menu.transform)
                    {
                        if (sub.gameObject.name == "Room")
                        {
                            foreach (Transform wall in sub.transform)
                            {
                                if (wall.name.Contains("Wall") && !ExistsInHierarchy(childData.children, wall.name))
                                {
                                    wall.gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static bool ExistsInHierarchy(List<ChildData> children, string name)
    {
        foreach (var child in children)
        {
            if (child.name == name)
                return true;
            if (child.children != null && ExistsInHierarchy(child.children, name))
                return true;
        }
        return false;
    }
}
