using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PlayerMoneyDisplay;
    PlayerStats playerStats;
    private void OnEnable()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        PlayerMoneyDisplay.text = $"{playerStats.Money}$";
    }
    private void Update()
    {
        PlayerMoneyDisplay.text = $"{playerStats.Money}$";
    }
}
