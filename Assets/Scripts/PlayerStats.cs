using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int Money { get; private set; } = 10000;

    private void OnEnable()
    {
        // Abonnez-vous � l'�v�nement OnFloorPlaced pour mettre � jour l'argent
        Bloc.OnFloorPlaced.AddListener(ChangeMoneyDown);
        Bloc.OnFloorRemoved.AddListener(ChangeMoneyUp);
    }
    private void OnDisable()
    {
        // D�sabonnez-vous de l'�v�nement OnFloorPlaced pour �viter les fuites de m�moire
        Bloc.OnFloorPlaced.RemoveListener(ChangeMoneyDown);
        Bloc.OnFloorRemoved.RemoveListener(ChangeMoneyUp);
    }
    // M�thode pour changer le montant d'argent
    public void ChangeMoneyUp(int amount)
    {
        Money += amount;
    }
    public void ChangeMoneyDown(int amount)
    {
        Money -= amount;
    }
}
