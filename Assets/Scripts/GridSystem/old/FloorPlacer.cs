using System.Collections.Generic;
using UnityEngine;

public class FloorPlacer : MonoBehaviour
{
    public GameObject floorPrefab;  // Le sol à placer
    public int gridWidth = 10;      // Largeur de la grille
    public int gridHeight = 10;     // Hauteur de la grille
    public float cellSize = 2f;     // Taille de la cellule


    public HashSet<Vector2Int> placedFloors = new HashSet<Vector2Int>();

    void Update()
    {
        // Vérifie si l'utilisateur a cliqué avec le bouton gauche de la souris
        if (Input.GetMouseButtonDown(0))
        {
            PlaceFloorAtMousePosition();
        }
    }

    void PlaceFloorAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = Mathf.FloorToInt(hit.point.x / cellSize);
            int z = Mathf.FloorToInt(hit.point.z / cellSize);

            Vector2Int cell = new Vector2Int(x, z);

            if (x >= 0 && x < gridWidth && z >= 0 && z < gridHeight && !placedFloors.Contains(cell))
            {
                placedFloors.Add(cell);
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
                Instantiate(floorPrefab, position, Quaternion.identity);
            }
        }
    }
}
