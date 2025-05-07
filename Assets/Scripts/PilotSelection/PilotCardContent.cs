using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PilotCardContent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PilotName, PilotLvl;
    [SerializeField] GameObject Lock;
    [SerializeField] Image PilotIcon;
    public Pilot Pilot;

    public void Unlock()
    {
        Lock.SetActive(false);
    }
    public void SetName(string name) 
    {
        PilotName.text = name;
    }
    public void SetIcon(Sprite icon)
    {
        PilotIcon.sprite = icon;
    }
    public void SetLevel(int lvl) 
    {
        PilotLvl.text = $"Level : {lvl}";
    }
}
