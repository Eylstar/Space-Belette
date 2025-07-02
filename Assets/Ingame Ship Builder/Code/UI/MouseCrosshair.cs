using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the position of this GameObject to reflect the position of the mouse
/// when the player ship is using mouse input. Otherwise, it just hides it.
/// </summary>
public class MouseCrosshair : MonoBehaviour
{
    public Image crosshair;

    private int ROTATION_SPEED = 120;
    private Image cursor;

    private void Awake()
    {
        cursor = GetComponent<Image>();
        if (crosshair == null)
        {
            Debug.LogError("Please assign the crosshair image to Mousecrosshair image");
        }
    }

    private void Update()
    {
        if (cursor != null && Ship.PlayerShip != null)
        {
            cursor.enabled = Ship.PlayerShip.UsingMouseInput;

            if (cursor.enabled)
            {
                cursor.transform.position = Input.mousePosition;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (Ship.PlayerShip.InSupercruise)
        {
            crosshair.transform.Rotate(new Vector3(0, 0, ROTATION_SPEED*Time.deltaTime));
        }
        else
        {
            crosshair.transform.rotation = Quaternion.identity;
        }
    }
}
