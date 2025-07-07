//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(GridManager))]
//public class GridManagerEditor : Editor
//{
//    private void OnSceneGUI()
//    {
//        GridManager grid = (GridManager)target;

//        if (!grid.showGridGizmos) return;

//        GUIStyle style = new GUIStyle();
//        style.normal.textColor = Color.white;
//        style.fontSize = 12;
//        style.alignment = TextAnchor.MiddleCenter;

//        for (int x = 0; x < grid.width; x++)
//        {
//            for (int y = 0; y < grid.height; y++)
//            {
//                Vector3 worldPos = grid.GridToWorld(x, y) + new Vector3(grid.cellSize / 2f, 0.1f, grid.cellSize / 2f);
//                Handles.Label(worldPos, $"[{x},{y}]", style);
//            }
//        }
//    }
//}