using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.SceneManagement;

public class TutoPanel : MonoBehaviour
{
    [SerializeField] List<GameObject> Tutolist = new();
    private int currentIndex = 0; // Index de l'élément actuellement actif
    [SerializeField] TextMeshProUGUI btnTxt; // Référence au texte du bouton

    public void NextTuto()
    {
        if (Tutolist.Count == 0) return; // Vérifie si la liste est vide

        // Si on est au dernier élément, changer de scène
        if (currentIndex == Tutolist.Count - 1)
        {
            SceneManager.LoadScene("ShipBuilder");
            return;
        }

        // Désactiver l'élément actuel
        Tutolist[currentIndex].SetActive(false);

        // Passer à l'élément suivant
        currentIndex++;

        // Activer le nouvel élément
        Tutolist[currentIndex].SetActive(true);

        // Mettre à jour le texte du bouton si on est au dernier élément
        if (currentIndex == Tutolist.Count - 1)
        {
            btnTxt.text = "Go !"; // Par exemple, changer "Next" en "Terminer"
        }
    }

    // Méthode pour revenir à l'élément précédent
    public void PreviousTuto()
    {
        if (Tutolist.Count == 0) return; // Vérifie si la liste est vide

        // Désactiver l'élément actuel
        Tutolist[currentIndex].SetActive(false);

        // Revenir à l'élément précédent
        currentIndex--;

        // Si on dépasse le début, revenir au premier élément
        if (currentIndex < 0) currentIndex = 0;

        // Activer le nouvel élément
        Tutolist[currentIndex].SetActive(true);

        // Remettre le texte du bouton "Next" à son état initial
        btnTxt.text = "Suivant";
    }

}
