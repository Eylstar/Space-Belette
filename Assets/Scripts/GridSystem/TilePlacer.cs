using System.Collections.Generic;
using UnityEngine;
using static Floor;

public class TilePlacer : MonoBehaviour
{
    public enum BlocType
    {
        Floor,
        Weapon,
        Engine
    }
    public GameObject floorPrefab;
    private GridManager grid;
    private GameObject highlightedTile;
    private Material originalMaterial;
    public Material highlightMaterial; // Matériau pour la surbrillance (rouge)
    public Material selectedMaterial; // Matériau pour la sélection (vert)
    public ShipConstruct shipConstruct;
    private bool isHighlightMode = false;
    // Dictionnaire pour suivre les tuiles posées
    public Dictionary<Vector2Int, GameObject> placedTiles = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        grid = GridManager.Instance;
    }

    void Update()
    {
        if (isHighlightMode)
        {
            HandleMouseOver();

            if (Input.GetMouseButtonDown(0)) // clic gauche
            {
                HandleHighlightClick();
            }
        }
        else
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
    }

    public void EnableHighlightMode()
    {
        isHighlightMode = !isHighlightMode;
        if (!isHighlightMode)
        {
            ResetHighlight();
        }
        Debug.Log($"Mode surbrillance activé : {isHighlightMode}");
    }

    private void HandleMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Tile") || hitObject.CompareTag("Floor"))
            {
                if (highlightedTile != hitObject)
                {
                    ResetHighlight();
                    highlightedTile = hitObject;
                    originalMaterial = highlightedTile.GetComponent<Renderer>().material;
                    highlightedTile.GetComponent<Renderer>().material = highlightMaterial;
                }
            }
            else
            {
                ResetHighlight();
            }
        }
        else
        {
            ResetHighlight();
        }
    }

    private void ResetHighlight()
    {
        if (highlightedTile != null)
        {
            highlightedTile.GetComponent<Renderer>().material = originalMaterial;
            highlightedTile = null;
        }
    }

    private void HandleHighlightClick()
    {
        if (highlightedTile != null)
        {
            Vector3 adjustedPosition = highlightedTile.transform.position - new Vector3(0.5f, -0.5f, 0);
            Vector2Int gridPos = grid.WorldToGrid(adjustedPosition);
            PlacePlayerShip(gridPos);

            // Confirmer la sélection en mettant la case en vert
            highlightedTile.GetComponent<Renderer>().material = selectedMaterial;

            // Quitter le mode de sélection
            isHighlightMode = false;
            highlightedTile = null; // Réinitialiser la case surlignée
        }
    }

    private void HandleLeftClick()
    {
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
                    GameObject element = Instantiate(floorPrefab, hitObject.transform);
                    element.transform.localPosition = new Vector3(0.5f, -0.5f, 0f);
                    element.name = $"Bloc{gridPos.x}{gridPos.y}";

                    var floorComp = element.GetComponent<Floor>();
                    if (floorComp != null)
                    {
                        floorComp.SetFloorName(element.name);
                        UpdateWalls(gridPos, floorComp);                
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

    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.Log("Right click");
        // On vérifie qu'on a cliqué sur quelque chose dans la scène
        if (Physics.Raycast(ray, out hit, 100f))
        {
            // Vérifier si l'objet touché est une tuile
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Floor"))
            {
                Floor floor = hitObject.GetComponent<Floor>();
                Debug.Log($"Floor name: {floor.FloorName}");
                // Vérifier si la tuile est occupée par un sol
                if (floor != null)
                {
                    string name = floor.FloorName;
                    if (name != null)
                    {
                        foreach (KeyValuePair<Vector2Int, GameObject> pair in placedTiles)
                        {
                            if (pair.Value.name == name)
                            { 
                                UpdateNeighborWallsAfterRemoval(pair.Key);
                                placedTiles.Remove(pair.Key);
                                Destroy(pair.Value);
                                Debug.Log($"Sol supprimé à {pair.Key}");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log($"Aucun sol à supprimer");
                }
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

    private void UpdateWalls(Vector2Int pos, Floor currentFloor)
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

                Floor neighborFloor = placedTiles[neighborPos].GetComponent<Floor>();
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
                Floor neighborFloor = neighborObj.GetComponent<Floor>();
                neighborFloor.SetWall(directions[i], true);
            }
        }
    }
}
