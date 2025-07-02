using System.Collections.Generic;
using UnityEngine;

public class ConstructionHull : MonoBehaviour
{
    public Dictionary<HullHardpoint, GameObject> MountedComponents { get; private set; }

    private void Awake()
    {
        MountedComponents = new Dictionary<HullHardpoint, GameObject>();
        foreach (var hardpoint in gameObject.GetComponentsInChildren<HullHardpoint>())
            MountedComponents.Add(hardpoint, null);
    }

    /// <summary>
    /// Deselect all other hardpoints
    /// </summary>
    public void OnHardpointSelected(HullHardpoint selectedHp)
    {
        foreach(HullHardpoint hardpoint in MountedComponents.Keys)
        {
            if(hardpoint != selectedHp)
            {
                hardpoint.SetSelected(false);
            }
        }
    }

    public void MountComponent(HullHardpoint hardpoint, GameObject component)
    {
        UnmountComponent(hardpoint);

        var mountedComponent = Instantiate(
            component,
            hardpoint.transform.position,
            hardpoint.transform.rotation,
            transform);

        MountedComponents[hardpoint] = mountedComponent;
    }

    public void UnmountComponent(HullHardpoint hardpoint)
    {
        Destroy(MountedComponents[hardpoint]);
        MountedComponents[hardpoint] = null;
    }

    public void UnmountComponent(GameObject component)
    {
        foreach (var pair in MountedComponents)
        {
            if (pair.Value == component)
            {
                Destroy(component);
                MountedComponents[pair.Key] = null;
                return;
            }
        }
        
    }
}
