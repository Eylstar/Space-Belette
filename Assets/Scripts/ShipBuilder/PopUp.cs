using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public TextMeshProUGUI ObjName, Description, Price;

    public void SetText(string objName, string description, string price)
    {
        ObjName.text = objName;
        Description.text = description;
        Price.text = price;
    }
}
