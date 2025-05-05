using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Bloc;

public class Floor : MonoBehaviour
{

    public BlocType blocType;
    public int ID;
    public Sprite icon;
    [SerializeField] int cost;
    
    [SerializeField] GameObject wallNorth;
    [SerializeField] GameObject wallSouth;
    [SerializeField] GameObject wallEast;
    [SerializeField] GameObject wallWest;
    [SerializeField] GameObject roof;
    
    
    public string FloorName { get; private set; }

    public static UnityEvent<int> OnFloorPlaced = new UnityEvent<int>();
    public static UnityEvent<int> OnFloorRemoved = new UnityEvent<int>();

    private void OnEnable()
    {
        OnFloorPlaced.Invoke(cost);
    }
    
    private void OnDestroy()
    {
        OnFloorRemoved.Invoke(cost);
    }
    
    public void SetFloorName(string name)
    {
        FloorName = name;
    }

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
    public void SetWall(Direction dir, bool active)
    {
        switch (dir)
        {
            case Direction.North: if (wallNorth) wallNorth.SetActive(active); break;
            case Direction.South: if (wallSouth) wallSouth.SetActive(active); break;
            case Direction.East: if (wallEast) wallEast.SetActive(active); break;
            case Direction.West: if (wallWest) wallWest.SetActive(active); break;
        }
    }
    public void SetRoof(bool active)
    {
        if (roof) roof.SetActive(active);
    }
}