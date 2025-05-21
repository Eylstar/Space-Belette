using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Bloc;

public class ShipConstruct : MonoBehaviour
{
    PlayerShipSO playerShipSO;
    public GameObject PlayerShip;
    GameObject gridObj;
    Pilot Pilot;
    FloorListSO floorListSO;
    EngineListSO engineListSO;
    WeaponListSO weaponListSO;
    bool isCockpitPlaced = false;
    int CockpitCount;
    bool isEnginePlaced = false;
    int EngineCount;
    public int MaxWeight = 0;
    public int CurrentWeight = 0;
    [SerializeField] Slider weightSlider;
    [SerializeField] TextMeshProUGUI errorText;
    string errorLogs;
    private void OnEnable()
    {
        PilotSelection.OnPilotSelected += OnPilotSelected;
        OnUtilPlaced.AddListener(UtilAdd);
        OnUtilRemoved.AddListener(UtilRemove);
        UpdateWeight();
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
            if (CockpitCount != 1)
            {
                isCockpitPlaced = false;

            }
        }
        else if (type == UtilityType.Engine)
        {
            isEnginePlaced = true;
            EngineCount++;
        }
    }
    private void OnPilotSelected(Pilot pilot)
    {
        Pilot = pilot;
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
        errorLogs = string.Empty;

        if (!isCockpitPlaced || !isEnginePlaced || !IsAllConnected() || CurrentWeight > MaxWeight) 
        {
            if (!isCockpitPlaced)
            {
                if (CockpitCount > 1)
                {
                    errorLogs += $"You can have only one Cockpit !\n";
                }
                else { errorLogs += $"No Cockpit !\n"; }
            }
            if (!isEnginePlaced) errorLogs += $"No Engine!\n";
            if (CurrentWeight > MaxWeight) errorLogs += $"Too heavy! Remove some blocs.\n";
            ShowError(2f);
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

        playerShip.AddComponent<Playershipmove>();
        ShipManager sm = playerShip.AddComponent<ShipManager>();
        sm.MainPilot = Pilot;
        LoadGameObjectData(playerShip, shipData.children, sm);

        // Centrer le vaisseau autour du milieu de PlayerShip
        CenterPlayerShip(playerShip);

        PlayerShip = playerShip;
        playerShip.AddComponent<ShipColliderSetup>();
        sm.SpawnPilot();
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
                        if (child.GetComponent<Bloc>().utilityType == UtilityType.Cockpit)
                        {
                            manager.shipSpawners.Add(ShipManager.ShipSpawner.Cockpit , child.GetComponent<Bloc>().PlayerSpawn);
                        }
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
        // Récupérer tous les blocs de la grille
        List<Bloc> allBlocs = new List<Bloc>();
        foreach (Transform tile in gridObj.transform)
        {
            Bloc bloc = tile.GetComponentInChildren<Bloc>();
            if (bloc != null)
                allBlocs.Add(bloc);
        }

        // Trouver le cockpit
        Bloc cockpit = allBlocs.Find(b => b.utilityType == Bloc.UtilityType.Cockpit);
        if (cockpit == null)
        {
            errorLogs += "Aucun cockpit trouvé pour le test de connexion.\n";
            return false;
        }

        // Vérification : aucun bloc devant le cockpit (même X, Y > cockpit.Y)
        foreach (Bloc b in allBlocs)
        {
            if (b.CoordGrid.x == cockpit.CoordGrid.x && b.CoordGrid.y > cockpit.CoordGrid.y)
            {
                errorLogs += $"{cockpit.name} ne peut pas avoir de bloc devant lui (Y > {cockpit.CoordGrid.y}).\n";
                return false;
            }
        }

        // BFS
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Bloc> queue = new Queue<Bloc>();
        queue.Enqueue(cockpit);
        visited.Add(cockpit.CoordGrid);

        while (queue.Count > 0)
        {
            Bloc current = queue.Dequeue();
            foreach (Bloc neighbor in GetValidNeighbors(current, allBlocs))
            {
                if (!visited.Contains(neighbor.CoordGrid))
                {
                    visited.Add(neighbor.CoordGrid);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Vérifier que tous les blocs sont atteints
        foreach (Bloc bloc in allBlocs)
        {
            if (!visited.Contains(bloc.CoordGrid))
            {
                errorLogs += $"Le bloc '{bloc.name}' n'est pas connecté au cockpit.\n";
                return false;
            }
        }
        return true;
    }
    // Retourne les voisins valides selon les contraintes métier
    private List<Bloc> GetValidNeighbors(Bloc bloc, List<Bloc> allBlocs)
    {
        List<Bloc> neighbors = new List<Bloc>();
        Vector2Int coord = bloc.CoordGrid;

        // Définir les directions standards
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down
        };

        // Contraintes spécifiques Cockpit (déjà vérifiées dans IsAllConnected)
        if (bloc.utilityType == Bloc.UtilityType.Cockpit)
        {
            // Seulement en bas
            Vector2Int down = coord + Vector2Int.down;
            Bloc downBloc = allBlocs.Find(b => b.CoordGrid == down);
            if (downBloc != null)
            {
                // Vérifier qu'il n'y a pas d'engine en diagonale/côtés
                Vector2Int[] diagonals = new Vector2Int[]
                {
                    coord + Vector2Int.left,
                    coord + Vector2Int.right,
                    coord + Vector2Int.up + Vector2Int.left,
                    coord + Vector2Int.up + Vector2Int.right,
                    coord + Vector2Int.down + Vector2Int.left,
                    coord + Vector2Int.down + Vector2Int.right
                };
                foreach (var diag in diagonals)
                {
                    Bloc diagBloc = allBlocs.Find(b => b.CoordGrid == diag && b.utilityType == Bloc.UtilityType.Engine);
                    if (diagBloc != null)
                    {
                        errorLogs += $"{bloc.name} ne peut pas avoir de Engine en diagonale.\n";
                        return neighbors;
                    }
                }
                neighbors.Add(downBloc);
            }
        }
        // Contraintes spécifiques Engine
        else if (bloc.utilityType == Bloc.UtilityType.Engine)
        {
            // Seulement en haut
            Vector2Int up = coord + Vector2Int.up;
            Bloc upBloc = allBlocs.Find(b => b.CoordGrid == up);
            if (upBloc != null)
            {
                // Vérification : aucun bloc derrière l'engine (même X, Y < engine.Y)
                foreach (Bloc b in allBlocs)
                {
                    if (b.CoordGrid.x == bloc.CoordGrid.x && b.CoordGrid.y < bloc.CoordGrid.y)
                    {
                        errorLogs += $"{bloc.name} ne peut pas avoir de bloc derrière lui (Y < {bloc.CoordGrid.y}).\n";
                        return neighbors;
                    }
                }
                neighbors.Add(upBloc);
            }
        }
        // Autres blocs : voisins standards
        else
        {
            foreach (var dir in directions)
            {
                Vector2Int nCoord = coord + dir;
                Bloc nBloc = allBlocs.Find(b => b.CoordGrid == nCoord);
                if (nBloc != null)
                    neighbors.Add(nBloc);
            }
        }
        return neighbors;
    }
    //private bool HasNeighbor(Vector2Int coord, Bloc bloc)
    //{
    //    // Générer les noms des voisins potentiels
    //    Vector2Int[] neighborCoords;

    //    if (bloc.utilityType != UtilityType.Null) 
    //    { 
    //        // Si le bloc a un UtilityType, appliquer des règles spécifiques
    //        switch (bloc.utilityType)
    //        {
    //            case UtilityType.Cockpit:
    //                neighborCoords = new Vector2Int[] { coord + Vector2Int.down }; // Le voisin doit être en bas
    //                List<Vector2Int> frontCoords = new List<Vector2Int>();
    //                GridManager gm = FindFirstObjectByType<GridManager>();

    //                for (int y = coord.y+1; y < gm.height; y++)
    //                {
    //                    frontCoords.Add(new Vector2Int(coord.x, y)); // Ajouter toutes les cases devant (ligne au-dessus)
    //                }

    //                foreach (Vector2Int frontCoord in frontCoords)
    //                {
    //                    foreach (Transform tile in gridObj.transform)
    //                    {
    //                        Bloc frontBloc = tile.GetComponentInChildren<Bloc>();
    //                        if (frontBloc != null && frontBloc.CoordGrid == frontCoord)
    //                        {
    //                            Debug.LogWarning($"{bloc.name} ne peut pas avoir de bloc devant lui aux coordonnées {frontCoord}.");
    //                            errorLogs += $"{bloc.name} ne peut pas avoir de bloc devant lui.\n";
    //                            return false;
    //                        }
    //                    }
    //                }

    //                // Vérifier qu'il n'y a pas de Engine en diagonale et sur les cotes
    //                Vector2Int[] diagonalCoords = new Vector2Int[]
    //                {
    //                coord + Vector2Int.left,
    //                coord + Vector2Int.right,
    //                coord + Vector2Int.up + Vector2Int.left,  // Diagonale avant gauche
    //                coord + Vector2Int.up + Vector2Int.right,  // Diagonale avant droite
    //                coord + Vector2Int.down + Vector2Int.left,  // Diagonale bas gauche
    //                coord + Vector2Int.down + Vector2Int.right  // Diagonale bas droite
    //                };

    //                foreach (Vector2Int diagonalCoord in diagonalCoords)
    //                {
    //                    foreach (Transform tile in gridObj.transform)
    //                    {
    //                        Bloc diagonalBloc = tile.GetComponentInChildren<Bloc>();
    //                        if (diagonalBloc != null && diagonalBloc.CoordGrid == diagonalCoord && diagonalBloc.utilityType == UtilityType.Engine)
    //                        {
    //                            Debug.LogWarning($"{bloc.name} ne peut pas avoir de Engine en diagonale aux coordonnées {diagonalCoord}.");
    //                            errorLogs += $"{bloc.name} ne peut pas avoir de Engine en diagonale.\n";
    //                            return false;
    //                        }
    //                    }
    //                }
    //                Debug.Log($"Bloc '{bloc.name}' (Cockpit) : recherche uniquement en bas ({coord + Vector2Int.down})");
    //                break;

    //            case UtilityType.Engine:

    //                neighborCoords = new Vector2Int[] { coord + Vector2Int.up }; // Le voisin doit être en haut

    //                List<Vector2Int> backCoords = new List<Vector2Int>();
    //                for (int y = coord.y - 1; y > 0 ; y--)
    //                {
    //                    backCoords.Add(new Vector2Int(coord.x, y)); // Ajouter toutes les cases derriere (ligne au-dessous)
    //                }
    //                foreach (Vector2Int backCoord in backCoords)
    //                {
    //                    foreach (Transform tile in gridObj.transform)
    //                    {
    //                        Bloc backBloc = tile.GetComponentInChildren<Bloc>();
    //                        if (backBloc != null && backBloc.CoordGrid == backCoord)
    //                        {
    //                            Debug.LogWarning($"{bloc.name} ne peut pas avoir de bloc derriere lui aux coordonnees {backCoord}.");
    //                            errorLogs += $"{bloc.name} ne peut pas avoir de bloc derriere lui.\n";
    //                            return false;
    //                        }
    //                    }
    //                }
    //                Debug.Log($"Bloc {bloc.name} : recherche uniquement en haut ({coord + Vector2Int.up})");
    //                break;

    //            default:
    //                Debug.LogWarning($"UtilityType inconnu pour le bloc {bloc.name} : {bloc.utilityType}");
    //                return false;
    //        }
    //    }
    //    else
    //    {
    //        // Sinon, vérifier tous les voisins (haut, bas, gauche, droite)
    //        neighborCoords = new Vector2Int[]
    //        {
    //            coord + Vector2Int.right, // Droite
    //            coord + Vector2Int.left,  // Gauche
    //            coord + Vector2Int.up,    // Haut
    //            coord + Vector2Int.down   // Bas
    //        };
    //        Debug.Log($"Bloc '{bloc.name}' : recherche de voisins standards ({string.Join(", ", neighborCoords)})");
    //    }

    //    // Vérifier si au moins un voisin existe dans la hiérarchie
    //    foreach (Vector2Int neighborCoord in neighborCoords)
    //    {
    //        foreach (Transform tile in gridObj.transform)
    //        {
    //            Bloc neighborBloc = tile.GetComponentInChildren<Bloc>();
    //            if (neighborBloc != null && neighborBloc.CoordGrid == neighborCoord)
    //            {
    //                Debug.Log($"Voisin trouvé pour '{bloc.name}' : Coordonnées {neighborCoord}");
    //                return true; // Un voisin valide a été trouvé
    //            }
    //        }
    //    }

    //    Debug.Log($"Aucun voisin trouvé pour le bloc '{bloc.name}' aux coordonnées ({coord})");
    //    return false; // Aucun voisin connecté trouvé
    //}

    internal void UpdateWeight()
    {
        //RectTransform sliderRT = weightSlider.GetComponent<RectTransform>();
        //sliderRT.sizeDelta = new Vector2(MaxWeight, sliderRT.sizeDelta.y);
        weightSlider.maxValue = MaxWeight;
        weightSlider.value = CurrentWeight;
    }
    public void ShowError(float duration = 2f)
    {
        if (errorText == null)
        {
            Debug.LogWarning("Aucun composant TMP_Text assigné pour afficher l'erreur.");
            return;
        }
        errorText.text = errorLogs;
        errorText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideError));
        Invoke(nameof(HideError), duration);
    }

    private void HideError()
    {
        if (errorText != null)
            errorText.gameObject.SetActive(false);
    }
}
