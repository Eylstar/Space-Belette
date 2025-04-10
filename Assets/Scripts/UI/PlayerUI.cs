using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PlayerMoneyDisplay;
    PlayerStats playerStats;
    private void OnEnable()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        PlayerMoneyDisplay.text = $"{playerStats.money}$";
    }
    private void Update()
    {
        PlayerMoneyDisplay.text = $"{playerStats.money}$";
    }
}
