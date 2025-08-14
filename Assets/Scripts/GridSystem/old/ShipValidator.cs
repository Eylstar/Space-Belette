//using System.Collections.Generic;
//using UnityEngine;

//public static class ShipValidator
//{
//    public static bool IsAllConnected(ShipConstruct context, out string errorLogs)
//    {
//        errorLogs = string.Empty;
//        var gridObj = context.gridObj;
//        List<Bloc> allBlocs = new List<Bloc>();
//        foreach (Transform tile in gridObj.transform)
//        {
//            Bloc bloc = tile.GetComponentInChildren<Bloc>();
//            if (bloc != null)
//                allBlocs.Add(bloc);
//        }

//        // Cockpit
//        Bloc cockpit = allBlocs.Find(b => b.utilityType == Bloc.UtilityType.Cockpit);
//        if (cockpit == null)
//        {
//            errorLogs += "Aucun cockpit trouvé pour le test de connexion.\n";
//            return false;
//        }
//        foreach (Bloc b in allBlocs)
//        {
//            if (b.CoordGrid.x == cockpit.CoordGrid.x && b.CoordGrid.y > cockpit.CoordGrid.y)
//            {
//                errorLogs += $"{cockpit.name} ne peut pas avoir de bloc devant lui (Y > {cockpit.CoordGrid.y}).\n";
//                return false;
//            }
//            if (b.CoordGrid.x == cockpit.CoordGrid.x && b.CoordGrid.y == cockpit.CoordGrid.y-1 && b.blocType == Bloc.BlocType.Weapon)
//            {
//                errorLogs += $"{cockpit.name} ne peut pas avoir de bloc Weapon à l'arrière (Y < {cockpit.CoordGrid.y}).\n";
//                return false;
//            }
//        }
//        // Engine
//        foreach (Bloc engine in allBlocs)
//        {
//            if (engine.utilityType == Bloc.UtilityType.Engine)
//            {
//                foreach (Bloc b in allBlocs)
//                {
//                    if (b.CoordGrid.x == engine.CoordGrid.x && b.CoordGrid.y < engine.CoordGrid.y)
//                    {
//                        errorLogs += $"{engine.name} ne peut pas avoir de bloc derrière lui (Y < {engine.CoordGrid.y}).\n";
//                        return false;
//                    }
//                }
//            }
//        }
//        // BFS
//        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
//        Queue<Bloc> queue = new Queue<Bloc>();
//        queue.Enqueue(cockpit);
//        visited.Add(cockpit.CoordGrid);

//        while (queue.Count > 0)
//        {
//            Bloc current = queue.Dequeue();
//            foreach (Bloc neighbor in GetValidNeighbors(current, allBlocs, ref errorLogs))
//            {
//                if (!visited.Contains(neighbor.CoordGrid))
//                {
//                    visited.Add(neighbor.CoordGrid);
//                    queue.Enqueue(neighbor);
//                }
//            }
//        }
//        foreach (Bloc bloc in allBlocs)
//        {
//            if (!visited.Contains(bloc.CoordGrid))
//            {
//                errorLogs += $"Le bloc '{bloc.name}' n'est pas connecté au cockpit.\n";
//                return false;
//            }
//        }
//        return true;
//    }

//    public static List<Bloc> GetValidNeighbors(Bloc bloc, List<Bloc> allBlocs, ref string errorLogs)
//    {
//        List<Bloc> neighbors = new List<Bloc>();
//        Vector2Int coord = bloc.CoordGrid;
//        Vector2Int[] directions = new Vector2Int[]
//        {
//            Vector2Int.right,
//            Vector2Int.left,
//            Vector2Int.up,
//            Vector2Int.down
//        };

//        if (bloc.utilityType == Bloc.UtilityType.Cockpit)
//        {
//            Vector2Int down = coord + Vector2Int.down;
//            Bloc downBloc = allBlocs.Find(b => b.CoordGrid == down);
//            if (downBloc != null)
//            {
//                Vector2Int[] diagonals = new Vector2Int[]
//                {
//                    coord + Vector2Int.left,
//                    coord + Vector2Int.right,
//                    coord + Vector2Int.up + Vector2Int.left,
//                    coord + Vector2Int.up + Vector2Int.right,
//                    coord + Vector2Int.down + Vector2Int.left,
//                    coord + Vector2Int.down + Vector2Int.right
//                };
//                foreach (var diag in diagonals)
//                {
//                    Bloc diagBloc = allBlocs.Find(b => b.CoordGrid == diag && b.utilityType == Bloc.UtilityType.Engine);
//                    if (diagBloc != null)
//                    {
//                        errorLogs += $"{bloc.name} ne peut pas avoir de Engine en diagonale ou sur le cote.\n";
//                        return neighbors;
//                    }
//                }
//                neighbors.Add(downBloc);
//            }
//        }
//        else if (bloc.utilityType == Bloc.UtilityType.Engine)
//        {
//            Vector2Int up = coord + Vector2Int.up;
//            Bloc upBloc = allBlocs.Find(b => b.CoordGrid == up);
//            if (upBloc != null)
//                neighbors.Add(upBloc);
//        }
//        else
//        {
//            foreach (var dir in directions)
//            {
//                Vector2Int nCoord = coord + dir;
//                Bloc nBloc = allBlocs.Find(b => b.CoordGrid == nCoord);
//                if (nBloc != null)
//                    neighbors.Add(nBloc);
//            }
//        }
//        return neighbors;
//    }
//}
