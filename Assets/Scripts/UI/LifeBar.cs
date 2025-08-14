using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lifeText;
    Slider lifeBarSlider;

    private void OnEnable()
    {
        lifeBarSlider = GetComponent<Slider>();
        ShipMove.OnLifeSetup += OnLifeSetup;
        ShipMove.OnLifeChange += OnLifeChange;
    }

    private void OnLifeChange(int obj)
    {
        lifeBarSlider.value = obj;
        if (lifeText != null) lifeText.text = obj.ToString();
        Debug.Log($"LifeBar updated with value: {obj}");
    }

    private void OnLifeSetup(int obj)
    {
        lifeBarSlider.maxValue = obj;
        lifeBarSlider.value = obj;
        if (lifeText != null) lifeText.text = obj.ToString();
        Debug.Log($"LifeBar setup with max value: {obj}");
    }

}
