using System.Collections.Generic;
using UnityEngine;
using static Bloc;

public class TilePlacer : MonoBehaviour
{
    public GameObject floorPrefab;
    private GridManager grid;
    public ShipConstruct shipConstruct;
    public Dictionary<Vector2Int, GameObject> placedTiles = new Dictionary<Vector2Int, GameObject>();
    PlayerStats playerStats;

    void Start()
    {
        grid = GridManager.Instance;
        shipConstruct = FindFirstObjectByType<ShipConstruct>();
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            HandleLeftClick();
        }
        else if (Input.GetMouseButtonDown(1)) // clic droit
        {
            HandleRightClick();
        }
    }

    private void HandleLeftClick()
    {
        if (CanBuyPart() == false) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Tile"))
            {
                Vector2Int gridPos = grid.WorldToGrid(hit.point);
                Vector3 centerWorld = grid.GridToWorld(gridPos.x, gridPos.y);
                Debug.Log($"Click @ {hit.point} → Grid {gridPos} → Center {centerWorld}");

                if (!placedTiles.ContainsKey(gridPos))
                {
                    GameObject element = Instantiate(floorPrefab, hitObject.transform.parent);
                    element.transform.localPosition = new Vector3(grid.cellSize*.5f,0f , -grid.cellSize * .5f);

                    var floorComp = element.GetComponent<Bloc>();
                    if (floorComp != null)
                    {
                        floorComp.CoordGrid = gridPos;
                        floorComp.SetBlocName(element.name);
                        UpdateWalls(gridPos, floorComp);
                        floorComp.SetRoof(false);
                        if (floorComp.BlocWeight > 0)
                        { shipConstruct.CurrentWeight += floorComp.BlocWeight; }
                        else { shipConstruct.MaxWeight += floorComp.WeightGain; }
                        shipConstruct.UpdateWeight();
                    }

                    placedTiles.Add(gridPos, element);
                }
            }
            else if (hitObject.CompareTag("Floor"))
            {
                Vector2Int gridPos = grid.WorldToGrid(hit.point);
                PlacePlayerShip(gridPos);
            }
        }
    }
    private bool CanBuyPart()
    {
        if (floorPrefab != null && playerStats.Money >= floorPrefab.GetComponent<Bloc>().Cost)
        {
            return true;
        }
        else if (floorPrefab == null)
        {
            Debug.Log("Aucune partie sélectionnée.");
            return false;
        }
        else
        {
            Debug.Log("Pas assez d'argent pour acheter cette partie.");
            return false;
        }
    }
    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Bloc"))
            {
                Bloc floor = hitObject.GetComponent<Bloc>();

                if (floor != null)
                {
                    Vector2Int gridPos = floor.CoordGrid; // Utilisation des coordonnées de la grille
                    if (placedTiles.ContainsKey(gridPos))
                    {
                        if (floor.BlocWeight > 0)
                        { shipConstruct.CurrentWeight -= floor.BlocWeight; }
                        else { shipConstruct.MaxWeight -= floor.WeightGain; }
                        UpdateNeighborWallsAfterRemoval(gridPos);
                        GameObject tileToRemove = placedTiles[gridPos];
                        placedTiles.Remove(gridPos);
                        Destroy(tileToRemove);
                        shipConstruct.UpdateWeight();
                        Debug.Log($"Bloc supprimé à {gridPos}");
                    }
                    else
                    {
                        Debug.LogWarning($"Aucun bloc trouvé à {gridPos} dans placedTiles.");
                    }
                }
                else
                {
                    Debug.LogWarning("Le GameObject cliqué n'a pas de composant Bloc.");
                }
            }
            else
            {
                Debug.Log("Le GameObject cliqué n'est pas un Bloc.");
            }
        }
    }

    private void PlacePlayerShip(Vector2Int gridPos)
    {
        if (shipConstruct.PlayerShip != null)
        {
            Vector3 centerWorld = grid.GridToWorld(gridPos.x, gridPos.y);
            shipConstruct.PlayerShip.transform.position = centerWorld;
            Debug.Log($"PlayerShip placé à {centerWorld}");
        }
    }

    private void UpdateWalls(Vector2Int pos, Bloc currentFloor)
    {
        Vector2Int[] offsets = {
            new Vector2Int(0, 1),  // North
            new Vector2Int(0, -1), // South
            new Vector2Int(1, 0),  // East
            new Vector2Int(-1, 0)  // West
        };

        Direction[] dirEnums = {
            Direction.North,
            Direction.South,
            Direction.East,
            Direction.West
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int neighborPos = pos + offsets[i];
            Direction dirTowardNeighbor = dirEnums[i];
            Direction dirFromNeighbor = Opposite(dirTowardNeighbor);

            if (placedTiles.ContainsKey(neighborPos))
            {
                // On a un voisin : on supprime LE MUR VERS le voisin
                currentFloor.SetWall(dirTowardNeighbor, false);

                Bloc neighborFloor = placedTiles[neighborPos].GetComponent<Bloc>();
                neighborFloor.SetWall(dirFromNeighbor, false);
            }
            else
            {
                // Pas de voisin : garde le mur actif
                currentFloor.SetWall(dirTowardNeighbor, true);
            }
        }
    }

    private Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => dir
        };
    }

    private void UpdateNeighborWallsAfterRemoval(Vector2Int removedPos)
    {
        Vector2Int[] offsets = {
            new Vector2Int(0, 1),  // North
            new Vector2Int(1, 0),  // East
            new Vector2Int(0, -1), // South
            new Vector2Int(-1, 0)  // West
        };

        Direction[] directions = {
            Direction.South,  // Pour voisin Nord, on remet son mur Sud
            Direction.West,   // Pour voisin Est, on remet son mur Ouest
            Direction.North,  // Pour voisin Sud, on remet son mur Nord
            Direction.East    // Pour voisin Ouest, on remet son mur Est
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int neighborPos = removedPos + offsets[i];

            if (placedTiles.ContainsKey(neighborPos))
            {
                GameObject neighborObj = placedTiles[neighborPos];
                Bloc neighborFloor = neighborObj.GetComponent<Bloc>();
                neighborFloor.SetWall(directions[i], true);
            }
        }
    }
}
