//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

///// <summary>
///// Updates text information based on player ship state.
///// </summary>
//public class UIController : MonoBehaviour
//{
//    [Header("Left panel")]
//    public Text Speed;
//    public Text Throttle;
//    public Text FlightMode;

//    [Header("Right panel")]
//    public GameObject MouseInstructions;
//    public GameObject KeyboardInstructions;

//    private void Start()
//    {
//        OnFlightModeChanged();
//    }

//    void Update()
//    {
//        if (Throttle != null)
//        {
//            Throttle.text = string.Format("Throttle\n{0}", (Ship.PlayerShip.Throttle * 100.0f).ToString("000"));
//        }
//        if (Speed != null)
//        {
//            Speed.text = string.Format("Velocity\n{0}", Ship.PlayerShip.Velocity.magnitude.ToString("000"));
//        }
//        if(FlightMode != null)
//        {
//            FlightMode.text = string.Format(
//                "Flight mode\n{0}",
//                Ship.PlayerShip.UsingMouseInput ? "Mouse flight" : "Keyboard flight");
//        }

//        if (Input.GetKeyUp(KeyCode.Space))
//        {
//            OnFlightModeChanged();
//        }

//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            SceneManager.LoadScene("Builder");
//        }
//    }

//    public void OnFlightModeChanged()
//    {
//        if(Ship.PlayerShip.UsingMouseInput)
//        {
//            MouseInstructions.SetActive(true);
//            KeyboardInstructions.SetActive(false);
//        }
//        else
//        {
//            MouseInstructions.SetActive(false);
//            KeyboardInstructions.SetActive(true);
//        }
//    }
//}
