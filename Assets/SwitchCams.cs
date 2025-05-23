using Unity.Cinemachine;
using UnityEngine;

public class SwitchCams : MonoBehaviour
{
    CameraManager cameraManager;
    ShipColliderSetup shipColliderSetup;
    
    CinemachineCamera playerCinemachineCam;
    CinemachineCamera shipCinemachineCam;
    
    
    GameObject player;
    bool isShipCamActive = true;
    private void Start()
    {
        cameraManager = FindFirstObjectByType<CameraManager>();
        playerCinemachineCam = cameraManager.playerCinemachineCam;
        shipCinemachineCam = cameraManager.shipCinemachineCam;
        
        shipColliderSetup = FindFirstObjectByType<ShipColliderSetup>();
        player = FindFirstObjectByType<PlayerMove>().gameObject;
        
        CameraTarget target = new CameraTarget();
        target.LookAtTarget = player.transform;
        target.TrackingTarget = player.transform;
        playerCinemachineCam.Target = target;
    }
    
    public void SwitchToShipCam()
    {;
        if (!shipColliderSetup)
            shipColliderSetup = FindFirstObjectByType<ShipColliderSetup>();
        
        playerCinemachineCam.enabled = false;
        shipCinemachineCam.enabled = true;
        shipColliderSetup.SetCollidersActive(true);
        GetComponent<ShipMove>().enabled = true;
        GetComponent<ShipShoot>().enabled = true;
        player.SetActive(false);
    }
    
    public void SwitchToPlayerCam()
    {
        if (!shipColliderSetup)
            shipColliderSetup = FindFirstObjectByType<ShipColliderSetup>();
        
        playerCinemachineCam.enabled = true;
        shipCinemachineCam.enabled = false;
        shipColliderSetup.SetCollidersActive(false);
        GetComponent<ShipMove>().enabled = false;
        GetComponent<ShipShoot>().enabled = false;
        player.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isShipCamActive = !isShipCamActive;
            if (isShipCamActive)
                SwitchToShipCam();
            else
                SwitchToPlayerCam();
        }
    }
}
