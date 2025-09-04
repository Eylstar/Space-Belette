using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] Image Icon;
    [SerializeField] TextMeshProUGUI Description;
    public ShipUpgradeSo upgradeContent;

    public void SetUpgrade(ShipUpgradeSo content) 
    {
        Icon.sprite = content.Icon;
        Description.text = content.UpgradeName;
        upgradeContent = content;
    }
}
