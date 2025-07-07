using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.SceneManagement;

public class TutoPanel : MonoBehaviour
{
    [SerializeField] List<GameObject> Tutolist = new();
    private int currentIndex = 0; // Index de l'�l�ment actuellement actif
    [SerializeField] TextMeshProUGUI btnTxt; // R�f�rence au texte du bouton

    public void NextTuto()
    {
        if (Tutolist.Count == 0) return; // V�rifie si la liste est vide

        // Si on est au dernier �l�ment, changer de sc�ne
        if (currentIndex == Tutolist.Count - 1)
        {
            SceneManager.LoadScene("ShipBuilder");
            return;
        }

        // D�sactiver l'�l�ment actuel
        Tutolist[currentIndex].SetActive(false);

        // Passer � l'�l�ment suivant
        currentIndex++;

        // Activer le nouvel �l�ment
        Tutolist[currentIndex].SetActive(true);

        // Mettre � jour le texte du bouton si on est au dernier �l�ment
        if (currentIndex == Tutolist.Count - 1)
        {
            btnTxt.text = "Go !"; // Par exemple, changer "Next" en "Terminer"
        }
    }

    // M�thode pour revenir � l'�l�ment pr�c�dent
    public void PreviousTuto()
    {
        if (Tutolist.Count == 0) return; // V�rifie si la liste est vide

        // D�sactiver l'�l�ment actuel
        Tutolist[currentIndex].SetActive(false);

        // Revenir � l'�l�ment pr�c�dent
        currentIndex--;

        // Si on d�passe le d�but, revenir au premier �l�ment
        if (currentIndex < 0) currentIndex = 0;

        // Activer le nouvel �l�ment
        Tutolist[currentIndex].SetActive(true);

        // Remettre le texte du bouton "Next" � son �tat initial
        btnTxt.text = "Suivant";
    }

}
