using UnityEngine;
using UnityEngine.UI;

public class GridToggleUI : MonoBehaviour
{
    public Button toggleButton;
    private bool isVisible = false;

    void Start()
    {
        //toggleButton.onClick.AddListener(ToggleGrid);
    }

    public void ToggleGrid()
    {
        isVisible = !isVisible;
        if (isVisible)
        {
            GridManager.Instance.ShowGrid(true);
        }
        else
        {
            GridManager.Instance.ShowGrid(false);
        }
    }
}
