using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.Events;

public class Bloc : MonoBehaviour
{
    public BlocType blocType;
    public UtilityType utilityType = UtilityType.Null;
    public Vector2Int CoordGrid;
    public int LifeBonus;
    public int BlocWeight = 0;
    public int WeightGain = 0;
    public string BlocName { get; private set; }
    public int ID;
    public Sprite Icon;
    public int Cost;
    [SerializedDictionary("Object", "Collider")]
    [SerializeField] SerializedDictionary<GameObject, GameObject> colliders = new SerializedDictionary<GameObject, GameObject>();
    
    public GameObject WallNorth, WallSouth, WallEast, WallWest, Roof;
    public Transform BulletSpawn;


    public static UnityEvent<int> OnFloorPlaced = new UnityEvent<int>();
    public static UnityEvent<int> OnFloorRemoved = new UnityEvent<int>();

    public static UnityEvent<UtilityType> OnUtilPlaced = new UnityEvent<UtilityType>();
    public static UnityEvent<UtilityType> OnUtilRemoved = new UnityEvent<UtilityType>();

    private void OnEnable()
    {
        OnFloorPlaced.Invoke(Cost);
        if (utilityType != UtilityType.Null)
        {
            OnUtilPlaced.Invoke(utilityType);
        }
    }

    private void OnDestroy()
    {
        OnFloorRemoved.Invoke(Cost);
        if (utilityType != UtilityType.Null)
        {
            OnUtilRemoved.Invoke(utilityType);
        }
    }

    public void SetBlocName(string name)
    {
        BlocName = name;
    }
    public void SetWall(Direction dir, bool active)
    {
        switch (dir)
        {
            case Direction.North: if (WallNorth) WallNorth.SetActive(active); break;
            case Direction.South: if (WallSouth) WallSouth.SetActive(active); break;
            case Direction.East: if (WallEast) WallEast.SetActive(active); break;
            case Direction.West: if (WallWest) WallWest.SetActive(active); break;
        }
    }
    public void SetRoof(bool active)
    {
        if (Roof) Roof.SetActive(active);
    }
    public enum BlocType
    {
        Floor,
        Utility,
        Weapon
    }
    public enum UtilityType
    {
        Cockpit,
        Engine,
        Other, 
        Null
    }
    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
