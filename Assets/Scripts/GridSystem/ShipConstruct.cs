using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Bloc;

public class ShipConstruct : MonoBehaviour
{
    PlayerShipSO playerShipSO;
    public GameObject PlayerShip;
    GameObject gridObj;
    GameObject playerCharacter;
    FloorListSO floorListSO;
    EngineListSO engineListSO;
    WeaponListSO weaponListSO;
    bool isCockpitPlaced = false;
    bool isEnginePlaced = false;

    private void OnEnable()
    {
        PilotSelection.OnPilotSelected += OnPilotSelected;
        OnUtilPlaced.AddListener(UtilAdd);
        OnUtilRemoved.AddListener(UtilRemove);
    }
    private void OnDisable()
    {
        PilotSelection.OnPilotSelected -= OnPilotSelected;
        OnUtilPlaced.RemoveListener(UtilAdd);
        OnUtilRemoved.RemoveListener(UtilRemove);
    }

    private void UtilRemove(UtilityType type)
    {
        if (type == UtilityType.Cockpit)
        {
            isCockpitPlaced = false;
        }
        else if (type == UtilityType.Engine)
        {
            isEnginePlaced = false;
        }
    }

    void UtilAdd(UtilityType type)
    {
        if (type == UtilityType.Cockpit)
        {
            isCockpitPlaced = true;
        }
        else if (type == UtilityType.Engine)
        {
            isEnginePlaced = true;
        }
    }
    private void OnPilotSelected(Pilot pilot)
    {
        playerCharacter = pilot.pilotPrefab;
        SceneManager.LoadScene("Game");
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        gridObj = FindFirstObjectByType<GridManager>().gameObject;
        playerShipSO = Resources.Load<PlayerShipSO>("ScriptableObjects/PlayerShipSO");
        floorListSO = Resources.Load<FloorListSO>("ScriptableObjects/FloorList");
        engineListSO = Resources.Load<EngineListSO>("ScriptableObjects/EngineList");
        weaponListSO = Resources.Load<WeaponListSO>("ScriptableObjects/WeaponList");
    }

    public void ReassignChildrenToPlayerShip()
    {
        if (!isCockpitPlaced || !isEnginePlaced || !IsAllConnected()) 
        {
            Debug.Log($"Cockpit : {isCockpitPlaced} / Engine : {isEnginePlaced} / All Connected : {IsAllConnected()}");
            return; 
        }
        
        // Parcourir tous les enfants du GameObject
        foreach (Transform tile in gridObj.transform)
        {
            Bloc bloc = tile.GetComponentInChildren<Bloc>();
            // Vérifier si l'enfant a des enfants
            if (bloc != null)
            {
                // Parcourir les enfants de l'enfant
                foreach (Transform menu in bloc.transform)
                {
                    foreach(Transform sub in menu.transform) 
                    {
                        if (sub.gameObject.name == "Room") 
                        {
                            foreach (Transform wall in sub.transform) 
                            {
                                if (!wall.gameObject.activeInHierarchy) 
                                {
                                    Destroy(wall.gameObject);
                                }
                            }
                        }
                    }
                }
                bloc.transform.SetParent(PlayerShip.transform);
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
        SceneManager.LoadScene("CharacterSelection");
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

    public void LoadPlayerShip()
    {
        if (playerShipSO.shipData == null)
        {
            Debug.LogError("Aucune donnée de vaisseau trouvée dans le ScriptableObject");
            return;
        }

        ShipData shipData = playerShipSO.shipData;
        GameObject playerShip = new GameObject(shipData.name);
        var rb = playerShip.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.freezeRotation = true;
        rb.useGravity = false;

        playerShip.AddComponent<Playershipmove>();

        LoadGameObjectData(playerShip, shipData.children);

        // Centrer le vaisseau autour du milieu de PlayerShip
        CenterPlayerShip(playerShip);

        PlayerShip = playerShip;
        playerShip.AddComponent<ShipColliderSetup>();
        Instantiate(playerCharacter, PlayerShip.transform.position, Quaternion.identity, PlayerShip.transform);
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
                case BlocType.Utility:
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
                SetWalls(childData, child);
            }
        }
    }
    void SetWalls(ChildData childData, GameObject child)
    { 
        // Désactiver les murs qui ne sont pas dans les données uniquement pour les objets de type Floor
        if (childData.type != BlocType.Utility)
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
                                // Vérifier si le mur n'est pas dans les données et le désactiver
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
    private bool ExistsInHierarchy(List<ChildData> children, string name)
    {
        foreach (var child in children)
        {
            if (child.name == name)
            {
                return true;
            }
            if (child.children != null && ExistsInHierarchy(child.children, name))
            {
                return true;
            }
        }
        return false;
    }
    private bool IsAllConnected()
    {
        foreach (Transform tile in gridObj.transform)
        {
            Bloc bloc = tile.GetComponentInChildren<Bloc>();
            if (bloc == null) continue;

            // Extraire les coordonnées du bloc à partir de son nom
            string[] parts = bloc.name.Replace("Bloc", "").Split('.');
            Debug.Log($"Traitement du bloc : {bloc.name} - Coordonnées extraites : {string.Join(", ", parts)}");

            if (parts.Length != 2 || !int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
            {
                Debug.LogError($"Nom du bloc invalide : {bloc.name}");
                return false;
            }

            // Vérifier si le bloc a au moins un voisin valide
            if (!HasNeighbor(x, y, bloc))
            {
                Debug.LogWarning($"Le bloc '{bloc.name}' n'est pas connecté.");
                return false;
            }
        }

        return true;
    }

    private bool HasNeighbor(int x, int y, Bloc bloc)
    {
        // Générer les noms des voisins potentiels
        string[] neighborNames;

        if (bloc.utilityType != UtilityType.Null) 
        { 
            // Si le bloc a un UtilityType, appliquer des règles spécifiques
            switch (bloc.utilityType)
            {
                case UtilityType.Cockpit:
                    neighborNames = new string[] { $"Bloc{x}.{y - 1}" }; // Le voisin doit être en bas
                    Debug.Log($"Bloc '{bloc.name}' (Cockpit) : recherche uniquement en bas ({x}, {y - 1})");
                    break;

                case UtilityType.Engine:
                    neighborNames = new string[] { $"Bloc{x}.{y + 1}" }; // Le voisin doit être en haut
                    Debug.Log($"Bloc '{bloc.name}' (Engine) : recherche uniquement en haut ({x}, {y + 1})");
                    break;

                default:
                    Debug.LogWarning($"UtilityType inconnu pour le bloc '{bloc.name}' : {bloc.utilityType}");
                    return false;
            }
        }
        else
        {
            // Sinon, vérifier tous les voisins (haut, bas, gauche, droite)
            neighborNames = new string[]
            {
                $"Bloc{x + 1}.{y}", // Droite
                $"Bloc{x - 1}.{y}", // Gauche
                $"Bloc{x}.{y + 1}", // Haut
                $"Bloc{x}.{y - 1}"  // Bas
            };
            Debug.Log($"Bloc '{bloc.name}' : recherche de voisins standards ({string.Join(", ", neighborNames)})");
        }

        // Vérifier si au moins un voisin existe dans la hiérarchie
        foreach (string neighborName in neighborNames)
        {
            foreach (Transform tile in gridObj.transform)
            {
                // Vérifier si la Tile contient un enfant Bloc avec le nom correspondant
                Bloc neighborBloc = tile.GetComponentInChildren<Bloc>();
                if (neighborBloc != null && neighborBloc.name == neighborName)
                {
                    Debug.Log($"Voisin trouvé pour '{bloc.name}' : {neighborName} dans la Tile '{tile.name}'");
                    return true; // Un voisin valide a été trouvé
                }
            }
        }

        Debug.Log($"Aucun voisin trouvé pour le bloc '{bloc.name}' aux coordonnées ({x}, {y})");
        return false; // Aucun voisin connecté trouvé
    }
}
