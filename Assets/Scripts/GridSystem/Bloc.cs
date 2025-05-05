using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.Events;

public class Bloc : MonoBehaviour
{
    public BlocType blocType;
    public string BlocName { get; private set; }
    public int ID;
    public Sprite Icon;
    public int Cost;
    [SerializedDictionary("Object", "Collider")]
    [SerializeField] SerializedDictionary<GameObject, GameObject> colliders = new SerializedDictionary<GameObject, GameObject>();
    
    [SerializeField] GameObject WallNorth, WallSouth, WallEast, WallWest, BulletSpawn, Roof;


    public static UnityEvent<int> OnFloorPlaced = new UnityEvent<int>();
    public static UnityEvent<int> OnFloorRemoved = new UnityEvent<int>();

    private void OnEnable()
    {
        OnFloorPlaced.Invoke(Cost);
    }

    private void OnDestroy()
    {
        OnFloorRemoved.Invoke(Cost);
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
        Engine,
        Weapon
    }
    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
