using UnityEngine;

public class PopUpSystem : MonoBehaviour
{
    [SerializeField] private PopUp popUp;

    private void Awake()
    {
        popUp = FindFirstObjectByType<PopUp>();
        popUp.gameObject.SetActive(false);
    }

    public void ShowPopUp(string name, string desc, string price)
    {
        popUp.SetText(name, desc, price);
        popUp.gameObject.SetActive(true);
    }

    public void HidePopUp()
    {
        popUp.gameObject.SetActive(false);
    }
}
