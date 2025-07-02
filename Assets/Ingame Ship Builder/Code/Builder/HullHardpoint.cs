using UnityEngine;

public class HullHardpoint : MonoBehaviour
{
    [HideInInspector]
    public bool IsSelected = false;

    private MeshRenderer rend;
    private ConstructionHull shipHull;

    private void Start()
    {
        // Copy material to only modify copy
        rend = GetComponent<MeshRenderer>();
        rend.materials[0] = new Material(GetComponent<MeshRenderer>().materials[0]);

        shipHull = GetComponentInParent<ConstructionHull>();
    }

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
        SetSelected(IsSelected);
        shipHull.OnHardpointSelected(this);
    }

    public void SetSelected(bool isSelected)
    {
        IsSelected = isSelected;

        Color color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, (IsSelected ? 256 : 92) / 256f);
        rend.material.SetColor("_Color", color);
    }
}
