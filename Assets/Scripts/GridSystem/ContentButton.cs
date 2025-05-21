using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ContentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI Name, descTxt;
    GameObject _prefab;
    [SerializeField] Image icon;
    Bloc bloc;
    [SerializeField] GameObject DescObj;
    

    private void Awake()
    {
        DescObj.SetActive(false);
    }
    public void SetContent(string name, GameObject prefab)
    {
        Name.text = name;
        _prefab = prefab;
        bloc = prefab.GetComponent<Bloc>();
        if (bloc != null && bloc.Icon != null)
        {
            icon.sprite = bloc.Icon;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }

        SetText();
        
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        FindFirstObjectByType<TilePlacer>().floorPrefab = _prefab;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DescObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescObj.SetActive(false);
    }
    void SetText()
    {
        descTxt.text = $"{Name.text} \n"; 
        if (bloc.blocType != Bloc.BlocType.Utility)
        {
            descTxt.text += $"Type: {bloc.blocType} \n";
        }
        
        if (bloc.utilityType != Bloc.UtilityType.Null)
        {
            descTxt.text += $"Type: {bloc.utilityType} \n";
        }

        descTxt.text += $"Level: {bloc.Level} \n" +
            $"Life Bonus: <color=green>{bloc.LifeBonus}</color> \n";
        
        if (bloc.utilityType == Bloc.UtilityType.Engine)
        {
            descTxt.text += $"Weight Gain: <color=green>{bloc.WeightGain}</color> \n";
        }
        else if (bloc.utilityType != Bloc.UtilityType.Engine)
        {
            descTxt.text += $"Bloc Weight: <color=red>{bloc.BlocWeight}</color> \n";
        }

        descTxt.text += $"Cost: <color=red>{bloc.Cost}</color> \n";
    }
}
