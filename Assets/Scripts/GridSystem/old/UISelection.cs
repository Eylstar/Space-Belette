using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


//using static Bloc;

public class UISelection : MonoBehaviour
{
    //TilePlacer tp;
    [SerializeField] Button Floors, Engine, Weapons;
    [SerializeField] RectTransform Content;
    [SerializeField] GameObject contentPrefab;
    FloorListSO FloorList;
    EngineListSO EngineList;
    WeaponListSO WeaponList;
    //private void Start()
    //{
    //    ClearContent();
    //    tp = FindFirstObjectByType<TilePlacer>();
    //    FloorList = Resources.Load<FloorListSO>("ScriptableObjects/FloorList");
    //    EngineList = Resources.Load<EngineListSO>("ScriptableObjects/EngineList");
    //    WeaponList = Resources.Load<WeaponListSO>("ScriptableObjects/WeaponList");
    //    Floors.onClick.AddListener(() => SetContent(BlocType.Floor));
    //    Engine.onClick.AddListener(() => SetContent(BlocType.Utility));
    //    Weapons.onClick.AddListener(() => SetContent(BlocType.Weapon));

    //}
    private void OnEnable()
    {
        ClearContent();
    }
    void ClearContent()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
    }
    //void SetContent(BlocType Type)
    //{
    //    ClearContent();
    //    switch (Type)
    //    {
    //        case BlocType.Floor:
    //            foreach (var item in FloorList.FloorList)
    //            {
    //                GameObject obj = Instantiate(contentPrefab, Content);
    //                obj.GetComponent<ContentButton>().SetContent(item.Value.name, item.Value);
    //            }
    //            break;
    //        case BlocType.Utility:
    //            foreach (var item in EngineList.EngineList)
    //            {
    //                GameObject obj = Instantiate(contentPrefab, Content);
    //                obj.GetComponent<ContentButton>().SetContent(item.Value.name, item.Value);
    //            }
    //            break;
    //        case BlocType.Weapon:
    //            foreach (var item in WeaponList.WeaponList)
    //            {
    //                GameObject obj = Instantiate(contentPrefab, Content);
    //                obj.GetComponent<ContentButton>().SetContent(item.Value.name, item.Value);
    //            }
    //            break;
    //    }
    //}

}
