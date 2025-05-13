using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int Money { get; private set; } = 1000;

    private void OnEnable()
    {
        // Abonnez-vous à l'événement OnFloorPlaced pour mettre à jour l'argent
        Bloc.OnFloorPlaced.AddListener(ChangeMoneyDown);
        Bloc.OnFloorRemoved.AddListener(ChangeMoneyUp);
    }
    private void OnDisable()
    {
        // Désabonnez-vous de l'événement OnFloorPlaced pour éviter les fuites de mémoire
        Bloc.OnFloorPlaced.RemoveListener(ChangeMoneyDown);
        Bloc.OnFloorRemoved.RemoveListener(ChangeMoneyUp);
    }
    // Méthode pour changer le montant d'argent
    public void ChangeMoneyUp(int amount)
    {
        Money += amount;
    }
    public void ChangeMoneyDown(int amount)
    {
        Money -= amount;
    }
}
