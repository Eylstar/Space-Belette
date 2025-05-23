using TMPro;
using UnityEngine;

public static class ShipUIConst
{
    public static void ShowError(TextMeshProUGUI errorText, string errorLogs, MonoBehaviour context, float duration = 2f)
    {
        if (errorText == null)
        {
            Debug.LogWarning("Aucun composant TMP_Text assign� pour afficher l'erreur.");
            return;
        }
        errorText.text = errorLogs;
        errorText.gameObject.SetActive(true);
        context.CancelInvoke(nameof(HideError));
        context.Invoke(nameof(HideError), duration);
    }

    private static void HideError()
    {
        // Cette m�thode doit �tre appel�e via MonoBehaviour.Invoke, donc elle doit exister dans le MonoBehaviour appelant.
        // Ici, on laisse la m�thode vide, car l'appel r�el se fait dans ShipConstruct.
    }
}
