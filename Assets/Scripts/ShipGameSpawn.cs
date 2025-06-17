using UnityEngine;

public class ShipGameSpawn : MonoBehaviour
{
    [SerializeField] Transform spawnTransform;
    ShipConstruct shipCons;
    
    private void Awake()
    {
        shipCons = FindFirstObjectByType<ShipConstruct>();
        shipCons.LoadPlayerShip();
        shipCons.PlayerShip.transform.position = spawnTransform.position;
        shipCons.PlayerShip.transform.rotation = spawnTransform.rotation;
        shipCons.PlayerShip.transform.parent = spawnTransform.transform;
        
        FindFirstObjectByType<PlayerMove>(FindObjectsInactive.Include).SetupWalking();
    }
}
