//using UnityEditor;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class GridManager : MonoBehaviour
//{
//    public static GridManager Instance;

//    public GameObject PlayerShip;

//    [Header("Grid Settings")]
//    public int width = 10;
//    public int height = 10;
//    public float cellSize = 1f;

//    [Header("Debug")]
//    public bool showGridGizmos = true;

//    public GameObject gridTilePrefab; // assigné dans l'inspecteur
//    private GameObject[,] gridVisuals;

//    private void Awake()
//    {
//        if(SceneManager.GetActiveScene().name != "Grid")
//        {
//            return;
//        }
//        // Singleton pour y acc�der facilement depuis d'autres scripts
//        if (Instance == null) Instance = this;
//        else Destroy(gameObject);
//        GenerateGridVisual();
//        ShowGrid(true);
//    }

//    // Convertit une position de grille (x, y) en position monde
//    public Vector3 GridToWorld(int x, int y)
//    {
//        return new Vector3(x * cellSize, 0f, y * cellSize);
//    }

//    // Convertit une position monde en position de grille (x, y)
//    public Vector2Int WorldToGrid(Vector3 worldPosition)
//    {
//        int x = Mathf.FloorToInt((worldPosition.x + (cellSize / 2f)) / cellSize);
//        int y = Mathf.FloorToInt((worldPosition.z + (cellSize / 2f)) / cellSize);
//        return new Vector2Int(x, y);
//    }

//    public void GenerateGridVisual()
//    {
//        gridVisuals = new GameObject[width, height];

//        for (int x = 0; x < width; x++)
//        {
//            for (int y = 0; y < height; y++)
//            {
//                Vector3 pos = GridToWorld(x, y);
//                Quaternion rotation = Quaternion.Euler(0f, 0f, 0f); // Rotation de 90 degrés sur l'axe X
//                GameObject tile = Instantiate(gridTilePrefab, pos, rotation, transform);
//                tile.name = $"Tile_{x}_{y}";
//                gridVisuals[x, y] = tile;
//            }
//        }
//    }

//    public void ShowGrid(bool show)
//    {
//        if (gridVisuals == null) return;

//        foreach (var tile in gridVisuals)
//        {
//            if (tile != null)
//                tile.SetActive(show);
//        }
//    }
//}
