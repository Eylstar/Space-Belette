using System.Collections.Generic;
using UnityEngine;
using static TilePlacer;

public class ShipConstruct : MonoBehaviour
{
    public PlayerShipSO playerShipSO;
    public GameObject PlayerShip;

    FloorListSO floorListSO;
    EngineListSO engineListSO;
    WeaponListSO weaponListSO;

    private void Start()
    {
        floorListSO = Resources.Load<FloorListSO>("ScriptableObjects/FloorList");
        engineListSO = Resources.Load<EngineListSO>("ScriptableObjects/EngineList");
        weaponListSO = Resources.Load<WeaponListSO>("ScriptableObjects/WeaponList");
    }

    public void ReassignChildrenToPlayerShip()
    {
        // Parcourir tous les enfants du GameObject
        foreach (Transform tile in transform)
        {
            // Vérifier si l'enfant a des enfants
            if (tile.childCount > 0)
            {
                // Parcourir les enfants de l'enfant
                foreach (Transform floor in tile)
                {
                    if (floor.gameObject.activeInHierarchy)
                    {
                        foreach (Transform walls in floor)
                        {
                            if (!walls.gameObject.activeInHierarchy) Destroy(walls.gameObject);
                        }
                        // Réassigner le parent de l'enfant à PlayerShip
                        floor.SetParent(PlayerShip.transform);
                    }

                }
            }
        }
        SavePlayerShip();
    }

    public void SavePlayerShip()
    {
        ShipData shipData = new ShipData { name = PlayerShip.name };
        SaveGameObjectData(PlayerShip, shipData.children);
        playerShipSO.shipData = shipData;
        Debug.Log("Données de PlayerShip sauvegardées dans le ScriptableObject");
    }

    private void SaveGameObjectData(GameObject gameObject, List<ChildData> childrenData)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (!child.gameObject.activeInHierarchy) continue; // Ignorer les objets inactifs
            ChildData childData = new ChildData
            {
                name = child.name,
                position = child.localPosition,
                rotation = child.localRotation,
                scale = child.localScale,
                children = new List<ChildData>() // Initialiser la liste des enfants
            };

            // Vérifier si l'objet a un composant Floor et sauvegarder le type de bloc et l'ID
            Floor floorComponent = child.GetComponent<Floor>();
            if (floorComponent != null)
            {
                childData.type = floorComponent.blocType;
                childData.ID = floorComponent.ID;
            }

            childrenData.Add(childData);
            SaveGameObjectData(child.gameObject, childData.children);
        }
    }

    public void LoadPlayerShip()
    {
        if (playerShipSO.shipData == null)
        {
            Debug.LogError("Aucune donnée de vaisseau trouvée dans le ScriptableObject");
            return;
        }

        ShipData shipData = playerShipSO.shipData;
        GameObject playerShip = new GameObject(shipData.name);
        LoadGameObjectData(playerShip, shipData.children);

        // Centrer le vaisseau autour du milieu de PlayerShip
        CenterPlayerShip(playerShip);

        PlayerShip = playerShip;

        Debug.Log("PlayerShip reconstruit à partir du ScriptableObject");
    }
    private void CenterPlayerShip(GameObject playerShip)
    {
        // Calculer le centre du vaisseau en tenant compte du centre des tuiles
        Vector3 center = Vector3.zero;
        int childCount = 0;

        foreach (Transform child in playerShip.transform)
        {
            // Ajuster la position pour utiliser le centre de la tuile
            Vector3 tileCenter = child.localPosition + new Vector3(0.5f, 0, 0.5f);
            center += tileCenter;
            childCount++;
        }

        if (childCount > 0)
        {
            center /= childCount;
        }

        // Ajuster le centre en fonction des décalages observés
        center += new Vector3(-3, 0, 2);

        // Déplacer tous les enfants pour centrer le vaisseau
        foreach (Transform child in playerShip.transform)
        {
            child.localPosition -= center;
        }
    }
    private void LoadGameObjectData(GameObject parent, List<ChildData> childrenData)
    {
        foreach (ChildData childData in childrenData)
        {
            GameObject child = null;
            switch (childData.type)
            {
                case BlocType.Floor:
                    if (floorListSO.FloorList.TryGetValue(childData.ID, out GameObject floorPrefab))
                    {
                        child = Instantiate(floorPrefab, parent.transform);
                    }
                    break;
                case BlocType.Engine:
                    if (engineListSO.EngineList.TryGetValue(childData.ID, out GameObject enginePrefab))
                    {
                        child = Instantiate(enginePrefab, parent.transform);
                    }
                    break;
                case BlocType.Weapon:
                    if (weaponListSO.WeaponList.TryGetValue(childData.ID, out GameObject weaponPrefab))
                    {
                        child = Instantiate(weaponPrefab, parent.transform);
                    }
                    break;
            }

            if (child != null)
            {
                child.transform.localPosition = childData.position;
                child.transform.localRotation = childData.rotation;
                child.transform.localScale = childData.scale;

                // Désactiver les murs qui ne sont pas dans les données uniquement pour les objets de type Floor
                if (childData.type != BlocType.Engine)
                {
                    foreach (Transform wall in child.transform)
                    {
                        if (wall.name.Contains("Wall") && !childData.children.Exists(c => c.name == wall.name))
                        {
                            wall.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
