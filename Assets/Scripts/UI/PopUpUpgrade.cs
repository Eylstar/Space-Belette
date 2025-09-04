using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PopUpUpgrade : MonoBehaviour
{
    [SerializeField] GameObject upgradePanel, upgradeCardPrefab;
    [SerializeField] Transform upgradeCardContent;
    public List<ShipUpgradeSo> shipUpgrades = new();
    public List<ShipUpgradeSo> ShipUpgradeWeapons = new();
    public static event Action<ShipUpgradeSo> UpgradeSelected;
    private void OnEnable()
    {
        Ship.MainPilot.UpgradeShip += ShowUpgradePanel;
        Ship.MainPilot.UpgradeWeapon += ShowUpgradePanelWeapon;
        upgradePanel.SetActive(false);
    }
    private void OnDisable()
    {
        Ship.MainPilot.UpgradeShip -= ShowUpgradePanel;
    }

    private void ShowUpgradePanel()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        SetUpgrade(shipUpgrades);
    }
    private void ShowUpgradePanelWeapon()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        SetUpgrade(ShipUpgradeWeapons);
    }
    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    void SetUpgrade(List<ShipUpgradeSo> list)
    {
        foreach (Transform child in upgradeCardContent)
            Destroy(child.gameObject);

        var availableUpgrades = new List<ShipUpgradeSo>(list);
        int count = Mathf.Min(4, availableUpgrades.Count);

        for (int i = 0; i < count; i++)
        {
            // Prend un upgrade au hasard
            int index = Random.Range(0, availableUpgrades.Count);
            var upgrade = availableUpgrades[index];
            availableUpgrades.RemoveAt(index);

            // Instancie la carte
            GameObject card = Instantiate(upgradeCardPrefab, upgradeCardContent);

            // Remplis la carte avec les infos de l'upgrade (exemple)
            var cardUI = card.GetComponent<UpgradeCardUI>();
            if (cardUI != null)
            {
                cardUI.SetUpgrade(upgrade);
            }
            card.GetComponent<Button>().onClick.AddListener(() =>
            {
                UpgradeSelected?.Invoke(cardUI.upgradeContent);
                HideUpgradePanel();
            });
        }
    }
}
