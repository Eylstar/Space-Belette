using TMPro;
using UnityEngine;

public class PropInfo : MonoBehaviour
{
    string PropName;
    int Cost;
    [SerializeField] TextMeshProUGUI PropNameText,PropCostText;

    public void SetPropInfo(string propName, int cost)
    {
        PropName = propName;
        Cost = cost;
        PropNameText.text = PropName;
        PropCostText.text = Cost.ToString();
    }
}
