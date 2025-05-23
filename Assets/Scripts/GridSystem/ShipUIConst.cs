using TMPro;
using UnityEngine;

public static class ShipUIConst
{
    public static void ShowError(TextMeshProUGUI errorText, string errorLogs, MonoBehaviour context, float duration = 2f)
    {
        if (errorText == null)
        {
            Debug.LogWarning("Aucun composant TMP_Text assigné pour afficher l'erreur.");
            return;
        }
        errorText.text = errorLogs;
        errorText.gameObject.SetActive(true);
        context.CancelInvoke(nameof(HideError));
        context.Invoke(nameof(HideError), duration);
    }

    private static void HideError()
    {
        // Cette méthode doit être appelée via MonoBehaviour.Invoke, donc elle doit exister dans le MonoBehaviour appelant.
        // Ici, on laisse la méthode vide, car l'appel réel se fait dans ShipConstruct.
    }
}
