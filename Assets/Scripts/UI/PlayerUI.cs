using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PlayerMoneyDisplay;
    PlayerStats playerStats;
    private void OnEnable()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        PlayerMoneyDisplay.text = $"{PlayerStats.player.Money}$";
    }
    private void Update()
    {
        PlayerMoneyDisplay.text = $"{PlayerStats.player.Money}$";
    }
}
