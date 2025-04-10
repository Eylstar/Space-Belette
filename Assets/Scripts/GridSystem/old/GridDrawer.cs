using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int gridWidth = 10;     // Largeur de la grille
    public int gridHeight = 10;    // Hauteur de la grille
    public float cellSize = 2f;    // Taille d'une cellule

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;  // Définir la couleur des lignes de la grille

        // Dessiner les lignes horizontales de la grille
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, 0, gridHeight * cellSize);
            Gizmos.DrawLine(start, end);
        }

        // Dessiner les lignes verticales de la grille
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = new Vector3(0, 0, z * cellSize);
            Vector3 end = new Vector3(gridWidth * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(start, end);
        }
    }
}
