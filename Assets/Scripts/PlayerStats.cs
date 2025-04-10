using System;
using System.Drawing;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public int money { get; private set; } = 1000000;

    private void OnEnable()
    {
        // Abonnez-vous � l'�v�nement OnFloorPlaced pour mettre � jour l'argent
        Floor.OnFloorPlaced.AddListener(ChangeMoneyDown);
        Floor.OnFloorRemoved.AddListener(ChangeMoneyUp);
    }

    // M�thode pour changer le montant d'argent
    public void ChangeMoneyUp(int amount)
    {
        money += amount;
    }
    public void ChangeMoneyDown(int amount)
    {
        money -= amount;
    }
}
