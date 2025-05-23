using System;
using System.Collections.Generic;
using System.Drawing;
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
    int CockpitCount;
    bool isEnginePlaced = false;
    int EngineCount;
    public int MaxWeight = 0;
    public int CurrentWeight = 0;

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
            CockpitCount--;
            if (CockpitCount != 1) isCockpitPlaced = false;
        }
        else if (type == UtilityType.Engine)
        {
            EngineCount--;
            if (EngineCount < 1) isEnginePlaced = false;
        }
    }

    void UtilAdd(UtilityType type)
    {
        if (type == UtilityType.Cockpit)
        {
            isCockpitPlaced = true;
            CockpitCount++;
            if (CockpitCount != 1) isCockpitPlaced = false;
        }
        else if (type == UtilityType.Engine)
        {
            isEnginePlaced = true;
            EngineCount++;
        }
    }
    private void OnPilotSelected(Pilot pilot)
    {
        playerCharacter = pilot.pilotPrefab;
        SceneManager.LoadScene("Space");
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
        if (!isCockpitPlaced || !isEnginePlaced || !IsAllConnected() || CurrentWeight > MaxWeight) 
        {
            Debug.Log($"Cockpit : {isCockpitPlaced} / Engine : {isEnginePlaced} / All Connected : {IsAllConnected()} / Weight : {CurrentWeight}/{MaxWeight}");
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
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        //playerShip.AddComponent<Playershipmove>();
        ShipManager sm = playerShip.AddComponent<ShipManager>();
        LoadGameObjectData(playerShip, shipData.children, sm);

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
    private void LoadGameObjectData(GameObject parent, List<ChildData> childrenData, ShipManager manager)
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
                        SetShipManager(child, manager);
                    }
                    break;
                case BlocType.Utility:
                    if (engineListSO.EngineList.TryGetValue(childData.ID, out GameObject enginePrefab))
                    {
                        child = Instantiate(enginePrefab, parent.transform);
                        SetShipManager(child, manager);
                    }
                    break;
                case BlocType.Weapon:
                    if (weaponListSO.WeaponList.TryGetValue(childData.ID, out GameObject weaponPrefab))
                    {
                        child = Instantiate(weaponPrefab, parent.transform);
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
    void SetShipManager(GameObject comp, ShipManager manager) 
    {
        Bloc cb = comp.GetComponent<Bloc>();
        manager.MaxLife += cb.LifeBonus;
        manager.CurrentLife += cb.LifeBonus;
        manager.ShipBlocs.Add(cb);
        if (cb.blocType == BlocType.Weapon || cb.utilityType == UtilityType.Cockpit)
        {
            manager.ShootBlocs.Add(cb);
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

            // Vérifier si le bloc a des coordonnées valides
            if (bloc.CoordGrid == null)
            {
                Debug.LogError($"Le bloc '{bloc.name}' n'a pas de CoordGrid défini.");
                return false;
            }

            // Vérifier si le bloc a au moins un voisin valide
            if (!HasNeighbor(bloc.CoordGrid, bloc))
            {
                Debug.LogWarning($"Le bloc '{bloc.name}' aux coordonnées ({bloc.CoordGrid}) n'est pas valide.");
                return false;
            }
        }

        return true;
    }

    private bool HasNeighbor(Vector2Int coord, Bloc bloc)
    {
        // Générer les noms des voisins potentiels
        Vector2Int[] neighborCoords;

        if (bloc.utilityType != UtilityType.Null) 
        { 
            // Si le bloc a un UtilityType, appliquer des règles spécifiques
            switch (bloc.utilityType)
            {
                case UtilityType.Cockpit:
                    neighborCoords = new Vector2Int[] { coord + Vector2Int.down }; // Le voisin doit être en bas
                    List<Vector2Int> frontCoords = new List<Vector2Int>();
                    GridManager gm = FindFirstObjectByType<GridManager>();

                    for (int y = coord.y+1; y < gm.height; y++) // Remplacez `gridWidth` par la largeur réelle de votre grille
                    {
                        frontCoords.Add(new Vector2Int(coord.x, y)); // Ajouter toutes les cases devant (ligne au-dessus)
                    }

                    foreach (Vector2Int frontCoord in frontCoords)
                    {
                        foreach (Transform tile in gridObj.transform)
                        {
                            Bloc frontBloc = tile.GetComponentInChildren<Bloc>();
                            if (frontBloc != null && frontBloc.CoordGrid == frontCoord)
                            {
                                Debug.LogWarning($"{bloc.name} ne peut pas avoir de bloc devant lui aux coordonnées {frontCoord}.");
                                return false;
                            }
                        }
                    }

                    // Vérifier qu'il n'y a pas de Engine en diagonale et sur les cotes
                    Vector2Int[] diagonalCoords = new Vector2Int[]
                    {
                    coord + Vector2Int.left,
                    coord + Vector2Int.right,
                    coord + Vector2Int.up + Vector2Int.left,  // Diagonale avant gauche
                    coord + Vector2Int.up + Vector2Int.right,  // Diagonale avant droite
                    coord + Vector2Int.down + Vector2Int.left,  // Diagonale bas gauche
                    coord + Vector2Int.down + Vector2Int.right  // Diagonale bas droite
                    };

                    foreach (Vector2Int diagonalCoord in diagonalCoords)
                    {
                        foreach (Transform tile in gridObj.transform)
                        {
                            Bloc diagonalBloc = tile.GetComponentInChildren<Bloc>();
                            if (diagonalBloc != null && diagonalBloc.CoordGrid == diagonalCoord && diagonalBloc.utilityType == UtilityType.Engine)
                            {
                                Debug.LogWarning($"{bloc.name} ne peut pas avoir de Engine en diagonale aux coordonnées {diagonalCoord}.");
                                return false;
                            }
                        }
                    }
                    Debug.Log($"Bloc '{bloc.name}' (Cockpit) : recherche uniquement en bas ({coord + Vector2Int.down})");
                    break;

                case UtilityType.Engine:
                    neighborCoords = new Vector2Int[] { coord + Vector2Int.up }; // Le voisin doit être en haut
                    Debug.Log($"Bloc {bloc.name} : recherche uniquement en haut ({coord + Vector2Int.up})");
                    break;

                default:
                    Debug.LogWarning($"UtilityType inconnu pour le bloc {bloc.name} : {bloc.utilityType}");
                    return false;
            }
        }
        else
        {
            // Sinon, vérifier tous les voisins (haut, bas, gauche, droite)
            neighborCoords = new Vector2Int[]
            {
                coord + Vector2Int.right, // Droite
                coord + Vector2Int.left,  // Gauche
                coord + Vector2Int.up,    // Haut
                coord + Vector2Int.down   // Bas
            };
            Debug.Log($"Bloc '{bloc.name}' : recherche de voisins standards ({string.Join(", ", neighborCoords)})");
        }

        // Vérifier si au moins un voisin existe dans la hiérarchie
        foreach (Vector2Int neighborCoord in neighborCoords)
        {
            foreach (Transform tile in gridObj.transform)
            {
                Bloc neighborBloc = tile.GetComponentInChildren<Bloc>();
                if (neighborBloc != null && neighborBloc.CoordGrid == neighborCoord)
                {
                    Debug.Log($"Voisin trouvé pour '{bloc.name}' : Coordonnées {neighborCoord}");
                    return true; // Un voisin valide a été trouvé
                }
            }
        }

        Debug.Log($"Aucun voisin trouvé pour le bloc '{bloc.name}' aux coordonnées ({coord})");
        return false; // Aucun voisin connecté trouvé
    }
}
